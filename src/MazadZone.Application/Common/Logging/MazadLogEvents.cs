
public static partial class MazadLogEvents
{
    public static class Global
    {
        public const int NotFound = 100;

    }

    public static class System
    {

    }
    
    public static class Sellers
    {
        public const int NotFound = 150;
        public const int VerifyDomainViolation = 151;
        public const int VerifySuccess = 152;
        public const int BankUpdateDomainViolation = 153;
        public const int BankUpdateSuccess = 154;
        public const int BecomeSellerDomainViolation = 155;
        public const int BecomeSellerSuccess = 156;

    }

    public static class Bidders
    {
        public const int NotFound = 1001;
        public const int Registered = 1002;
        public const int ProfileError = 1003;
        public const int BidderRetrieved = 1002;
        public const int BidderProfileUpdated = 1003;

    }

    public static class Authentication
    {
        // Refresh Token Events
        public const int InvalidRefreshTokenProvided = 2001;
        public const int TokenRotationRejected = 2002;
        public const int TokenRefreshedSuccess = 2003;

        // Login Events (For when you build the Login Handler)
        public const int LoginSuccess = 2010;
        public const int InvalidCredentials = 2011;
        public const int AccountLockedOut = 2012;

        // General Auth
        public const int SessionInvalidated = 2020;

        public const int LogoutSuccess = 2021;
    public const int LogoutFromAllDevices = 2022;
    public const int LogoutSessionNotFound = 2023;


    }


}