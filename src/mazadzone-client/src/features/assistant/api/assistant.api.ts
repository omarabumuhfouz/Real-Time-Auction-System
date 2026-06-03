/**
 * Pure HTTP REST functions for the AI Sales Agent (Mazad Assistant).
 * Connects directly to the ASP.NET Core endpoints.
 */

import { api } from "@/lib/api/client";
import type { ChatAgentResponse } from "./assistant.contracts";

/**
 * Sends a message to the RAG-powered sales agent.
 */
export async function sendChatMessage(message: string): Promise<ChatAgentResponse> {
  const response = await api.post<ChatAgentResponse>("/chat/messages", { message });
  return response.data;
}
