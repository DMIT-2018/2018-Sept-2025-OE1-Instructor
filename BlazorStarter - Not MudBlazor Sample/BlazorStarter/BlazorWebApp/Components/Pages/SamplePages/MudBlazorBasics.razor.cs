namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class MudBlazorBasics
    {

        #region Random Number
        private int oddEvenValue;
        #endregion

        #region Text Boxes
        private string emailText = string.Empty;
        private string passwordText = string.Empty;
        //When using MudBlazor make sure all DateTimes are nullable (add the ?)
        private DateTime? dateText = DateTime.Today;
        #endregion

        #region Properties
        /// <summary>
        /// Returns a bool value (true/false) depending on if the value set in oddEvenValue is even or not
        /// </summary>
        //private bool IsEven
        //{
        //    get
        //    {
        //        return oddEvenValue % 2 == 0;
        //    }
        //}
        // Can be written as a simplified return
        // Example:
        private bool IsEven => oddEvenValue % 2 == 0;

        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            RandomValue();
        }

        private void RandomValue()
        {
            //Create an instance of the Random Class
            Random rnd = new();

            oddEvenValue = rnd.Next(0, 25);
        }
        #endregion
    }
}
