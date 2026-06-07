export const paymentKeys = {
  all: ["payment"] as const,
  savedMethods: () => [...paymentKeys.all, "saved-methods"] as const,
};
