/**
 * API Request and Response shapes for the AI Sales Agent (Mazad Assistant).
 * Aligned with the ASP.NET Core API OpenAPI specification.
 */

export interface SendChatMessageRequest {
  message: string;
}

export interface ChatAgentResponse {
  response: string;
}
