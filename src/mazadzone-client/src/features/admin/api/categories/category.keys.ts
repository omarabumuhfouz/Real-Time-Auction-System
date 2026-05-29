export const categoryKeys = {
  all: ["admin", "categories"] as const,
  tree: () => [...categoryKeys.all] as const,
  detail: (id: string) => [...categoryKeys.all, id] as const,
};
