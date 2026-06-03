/**
 * Public barrel exports for the AI Sales Agent (Mazad Assistant) feature.
 */

export { AssistantPopover } from "./components/AssistantPopover";
export { AssistantChatWindow } from "./components/AssistantChatWindow";
export { useSendChatMessage } from "./api/assistant.queries";
export { sendChatMessage } from "./api/assistant.api";
export type { SendChatMessageRequest, ChatAgentResponse } from "./api/assistant.contracts";
