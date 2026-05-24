"use client";

import * as signalR from "@microsoft/signalr";
import { env } from "@/config/env";
import { APP_CONFIG } from "@/config/app.config";

/**
 * Cache for active starting promises.
 * Prevents concurrent, rapid calls to connection.start() (e.g., during React 19 strict-mode 
 * double-mount sequences or immediate concurrent renders) from throwing 
 * "Cannot start a HubConnection that is not in the 'Disconnected' state." errors.
 */
const startingPromises = new WeakMap<signalR.HubConnection, Promise<void>>();

/**
 * Helper utility to concatenate base URLs and paths while eliminating 
 * potential duplicate or missing forward slashes.
 */
function joinUrl(baseUrl: string, path: string): string {
  return `${baseUrl.replace(/\/+$/, "")}/${path.replace(/^\/+/, "")}`;
}

/**
 * Creates and configures a new SignalR HubConnection instance.
 * 
 * @param hubPath - The relative path endpoint for the target Hub (e.g., "hubs/notifications")
 * @param accessTokenFactory - Optional function returning an access token string (or Promise of one)
 */
export function createHubConnection(
  hubPath: string,
  accessTokenFactory?: () => string | Promise<string>,
): signalR.HubConnection {
  const hubUrl = joinUrl(env.NEXT_PUBLIC_SIGNALR_HUB_URL, hubPath);
  const options: signalR.IHttpConnectionOptions = {};

  if (accessTokenFactory) {
    options.accessTokenFactory = async () => {
      const token = await accessTokenFactory();
      /**
       * Returning a blank string "" tells the Microsoft SignalR library to omit 
       * the "Authorization" header entirely from the handshake request.
       * This supports seamless anonymous guest connections to public feeds.
       */
      return token || "";
    };
  }

  return new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, options)
    .withAutomaticReconnect({
      /**
       * Exponential backoff reconnect interval schedule:
       * 0s, 2s, 5s, 10s, then retry every 30s.
       */
      nextRetryDelayInMilliseconds: (retryContext) => {
        const delays = [0, 2000, 5000, 10_000, 30_000];

        return delays[
          Math.min(retryContext.previousRetryCount, delays.length - 1)
        ];
      },
    })
    .configureLogging(
      process.env.NODE_ENV === "development"
        ? signalR.LogLevel.Information
        : signalR.LogLevel.Warning,
    )
    .build();
}

/**
 * Safe, synchronized method to start a SignalR connection.
 * Incorporates active starting promise caches, connection state checks, 
 * and handles reconnection states gracefully.
 */
export async function startConnection(
  connection: signalR.HubConnection,
): Promise<void> {
  // Check global realtime feature toggle before initiating connections
  if (!APP_CONFIG.enableRealtime) {
    return;
  }

  // If a starting process is already in flight, return its active promise
  const existingStart = startingPromises.get(connection);
  if (existingStart) {
    return existingStart;
  }

  // Already connected; resolve immediately
  if (connection.state === signalR.HubConnectionState.Connected) {
    return;
  }

  // Currently reconnecting natively; resolve early instead of crashing with a hard error
  if (connection.state === signalR.HubConnectionState.Reconnecting) {
    return;
  }

  // Wrap connection startup in a cached promise to prevent concurrent collision
  const startPromise = (async () => {
    try {
      await connection.start();
      console.log(`[SignalR] Connected to ${connection.baseUrl}`);
    } catch (err) {
      console.warn(`[SignalR] Connection failed for ${connection.baseUrl}:`, err);
      throw err;
    } finally {
      // Clear the starting cache once the startup cycle completes
      startingPromises.delete(connection);
    }
  })();

  startingPromises.set(connection, startPromise);

  return startPromise;
}

/**
 * Safe, synchronized method to stop a SignalR connection.
 * Awaits any in-progress starting promise before closing, preventing socket collisions.
 */
export async function stopConnection(
  connection: signalR.HubConnection,
): Promise<void> {
  const existingStart = startingPromises.get(connection);

  // If connection is in the middle of starting, wait for it to complete/fail before stopping
  if (existingStart) {
    try {
      await existingStart;
    } catch {
      // Ignore start failure; continue to safe cleanup teardown
    }
  }

  // Already disconnected; resolve immediately
  if (connection.state === signalR.HubConnectionState.Disconnected) {
    return;
  }

  try {
    await connection.stop();
    console.log(`[SignalR] Disconnected from ${connection.baseUrl}`);
  } catch (err) {
    console.error("[SignalR] Disconnect failed:", err);
  }
}
