export interface UserDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  role: string;
  status: string;
  joinedAt: string;
  lastLogin: string;
}

export interface PagedListOfUserDto {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  items: UserDto[];
}
