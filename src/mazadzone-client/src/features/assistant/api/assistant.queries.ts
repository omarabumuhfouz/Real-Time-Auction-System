/**
 * React Query hooks for the AI Sales Agent (Mazad Assistant).
 */

import { useMutation } from "@tanstack/react-query";
import { sendChatMessage } from "./assistant.api";
import type { ChatAgentResponse } from "./assistant.contracts";

/**
 * Mutation hook to send a message to the AI Sales Agent.
 */
export function useSendChatMessage() {
  return useMutation<ChatAgentResponse, Error, string>({
    mutationFn: async (message: string) => {
      return sendChatMessage(message);
    },
  });
}
