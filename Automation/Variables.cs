namespace Automation
{
    class Variables
    {
        public static string userName, selectedUserControl, confirmationNumber;
        public static bool isAdmin, justSignedOut, saved;

        public static string UserName { get => userName; set => userName = value; }
        public static string SelectedUserControl { get => selectedUserControl; set => selectedUserControl = value; }
        public static string ConfirmationNumber { get => confirmationNumber; set => confirmationNumber = value; }

        public static bool JustSignedOut { get => justSignedOut; set => justSignedOut = value; }
        public static bool Saved { get => saved; set => saved = value; }
        public static bool IsAdmin { get => isAdmin; set => isAdmin = value; }
    }
}
