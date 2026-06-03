export const usersKeys = {
  all: ["admin", "moderate-users"] as const,
  list: (filters: any) => [...usersKeys.all, filters] as const,
};
