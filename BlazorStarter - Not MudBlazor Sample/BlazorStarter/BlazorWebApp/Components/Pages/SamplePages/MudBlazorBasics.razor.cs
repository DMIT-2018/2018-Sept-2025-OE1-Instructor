using MudBlazor;

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
        //For Form Validation
        MudForm textForm = new();
        bool textIsValid;
        private string feedback = string.Empty;
        #endregion

        #region Radio Buttons, Checkboxes, & Text Area
        // Holds the array (represented by []) of string values used
        // to represent the various meal choices.
        private string[] meals = ["breakfast", "second breakfast", "lunch", "dinner"];

        // Used to hold the selected meal value for the MudBlazor component based Radio Group
        private string mudMeal = "breakfast";
        // Used to hold the value (results) of the checkbox
        // Note: Remember bool always initializes as false
        private bool acceptanceBox;
        // Used to hold the text area value
        private string messageBody = string.Empty;
        #endregion

        #region Lists & Sliders
        // Used to hold a posible collection of values
        // representing possible rides
        // -------------------------------
        // A Dictionary is a collection that represents
        // a key and a value, you can define the datatype for both.
        // In this example the key is an int and the value is a string
        // -------------------------------
        // Pretend this is a collection from a database 
        // The data to populate this Dictionary 
        // will be created in a separate method
        private Dictionary<int, string> rides = [];
        // Used to hold the selected value from the rides collection
        // Note: To not show the 0 when the page loads, use a nullable int.
        private int? myRide;
        // Used to hold a possible list of string
        // representing various vacation spots
        private List<string> vacationSpots = [];
        private IEnumerable<string> selectedVacationSpots = [];
        // Used to store the user's selected vacation spot.
        private string vacationSpot = string.Empty;
        // Used to hold the rating Value
        private int reviewRating = 5;
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
            PopulateCollections();
        }

        private void RandomValue()
        {
            //Create an instance of the Random Class
            Random rnd = new();

            oddEvenValue = rnd.Next(0, 25);
        }
        private void TextSubmit()
        {
            //force the form validation
            textForm.Validate();
            if (textIsValid)
            {
                feedback = $"Email: {emailText}; Password: {passwordText}; Date: {(dateText.HasValue ? dateText.Value.ToString("d") : "No Date")}";
            }
            else
            {
                feedback = "Text form is not valid!";
            }
        }
        /// <summary>
        /// Method is called when the user submits the radio, checkbox,
        /// and text area to update the resulting feedback.
        /// </summary>
        private void RadioCheckAreaSubmit()
        {
            feedback = $"Meal: {mudMeal}; Acceptance: {acceptanceBox}; Message: {messageBody}";
        }
        private void PopulateCollections()
        {
            int i = 1;
            
            // Populates the rides collection with values
            rides.Add(i++, "Car");
            rides.Add(i++, "Bus");
            rides.Add(i++, "Bike");
            rides.Add(i++, "Motorcycle");
            rides.Add(i++, "Boat");
            rides.Add(i++, "Plane");

            // Sort the 'ride' alphabetically based on the Value.
            rides.OrderBy(x => x.Value).ToDictionary();

            // Populates the vacationSpots List
            vacationSpots.Add("California");
            vacationSpots.Add("Caribbean");
            vacationSpots.Add("Cruising");
            vacationSpots.Add("Europe");
            vacationSpots.Add("Florida");
            vacationSpots.Add("Mexico");
        }
        /// <summary>
        /// Method is called when the user submits the lists and slider inputs to update the resulting feedback.
        /// </summary>
        private void ListSliderSubmit()
        {
            //Generate the feedback string incorporating the selected values
            feedback = $"Ride: {(myRide.HasValue ? rides[myRide.Value]:"No Ride Selected")}; Vacation Spot: {vacationSpot}; Review Rating: {reviewRating}";
        }
        #endregion
    }
}
