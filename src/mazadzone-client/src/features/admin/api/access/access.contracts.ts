/**
 * API Contracts for Access Management.
 * Aligned with backend CreateAdminUserCommand OpenAPI schema.
 */

export interface CreateAdminUserCommand {
  firstName: string;
  secondName: string;
  thirdName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  password: string;
}
