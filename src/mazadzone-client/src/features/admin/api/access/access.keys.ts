export const accessKeys = {
  all: ["admin", "access"] as const,
  list: (filters: any) => [...accessKeys.all, "list", filters] as const,
};
