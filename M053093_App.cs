/* Program:     Assessment Tracker
 * Description: A console application that allows the user to add/delete, 
 *              view assessments and check there due date and if they are due or overdue.
 *              The app will keep track of assessment results with the data saved to a text file
 * 
 *              
 * Author:      Russell Caine
 * Date:        5/03/2026
 * Version:     1.1 Finished draft
 */


using System.ComponentModel.Design;
using System.Xml.Linq;

namespace Assessment_Tracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //declare our file to store data
            string textFile = "results.txt";    

            //load existing results from the text file
            List<string> results = LoadResultsFromFile(textFile);

            //to exit the loop
            bool exit = false;

            //while exit == false     //while loop to keep the program running until the user decides to exit
            while (!exit)
            {
                //clear all
                Console.Clear();
                //Display menu options
                Console.WriteLine(" ===== Assessment Tracker ===== ");
                Console.WriteLine("\nMenu");
                Console.WriteLine("1. Display Results");       //DisplayResults method
                Console.WriteLine("2. Add Assessments");    //AddResult method
                Console.WriteLine("3. Delete Assessment");     //DeleteAssessment method
                Console.WriteLine("4. View Assessment List");   //ViewAllResults method
                Console.WriteLine("5. Exit");                   //Exit the program
                Console.WriteLine("Select an option: ");
                string input = Console.ReadLine();

                switch (input) 
                {
                    //if the user selects option 1, call the ViewResults method to display the results
                    case "1":
                        DisplayResults(results);
                        Pause("Press any key to continue...");
                        break;
                    //if the user selects option 2
                    case "2":
                        AddAssessment(results, textFile);
                        break;
                    //if the user selects option 3
                    case "3":
                        DeleteAssessment(results, textFile);
                        break;
                    //if user selects option 4
                    case "4":
                        ViewAssessmentList(results);
                        break;
                    //if the user selects option 5 the application will exit
                    case "5":
                        exit = true;
                        break;
                    //if the user enters invalid input
                    default:
                        Console.WriteLine("Invalid option. Please try again.");

                        Pause("Press any key to continue...");
                        break;
                }//end switch

            }//end while
        }//end main

        //Read the results from the file and return them as a list
        static List<string> LoadResultsFromFile(string filePath)
        {
            List<string> resultList = new List<string>();
            if (File.Exists(filePath))
            {
                //create a StreamReader object
                using (StreamReader reader = new StreamReader(filePath))
                {
                    //keep going until end of file
                    while (!reader.EndOfStream)
                    {
                        //read each line
                        string line = reader.ReadLine();
                        //if there is data on the line
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            //Add results as item to out list 
                            resultList.Add(line);
                        }
                    }



                }
            }
            return resultList;
        }// end LoadResultsFromFile


        //Displayresults method
        // display the list of assessment results if they exist
        static void DisplayResults(List<string> results)
        {
            //if the list is empty
            if (results.Count == 0)
            {
                Console.WriteLine("No Assessments In List");
            }
            else
            {
                ////loop through the list 
                for (int i = 0; i < results.Count; i++)     
                {
                    //format as name|dueDate|result|status                                   
                    string[] parts = results[i].Split('|'); //split on pipe (|)

                    string name = "";
                    string dueDateString = "";
                    string result = "";


                    if (parts.Length >= 3)
                    {
                        name = parts[0];
                        dueDateString = parts[1];
                        result = parts[2];

                        //Parse the due date to compare to current date
                        if (DateTime.TryParse(dueDateString, out DateTime dueDate))
                        {
                            string status;

                            if (dueDate.Date < DateTime.Today)
                                status = "Overdue";
                            else
                                status = "Due";

                            Console.WriteLine($"{i + 1}. Assessment Name: {name}, Date Due: {dueDate:dd/MM/yyyy}, Result: {result}, Status: {status}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{i + 1}. Name: {name}, Due: INVALID DATE, Result: {result}, Status: Unknown");
                    }

                }
            }
        } //end display results

        //write the current list of assessments back to the file using StreamWriter
        static void WriteResultsToFile(List<string> resultsList, string filePath)
        {
            //create a StreamWriter object
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string results in resultsList)
                {
                    writer.WriteLine(results);
                }
            }
        } //end WriteResultsToFile

        //AddAssessment method
        static void AddAssessment(List<string> results, string filePath)
        {
            //Ask user for assessment name
            Console.WriteLine("Enter assessment name: ");
            string? n = Console.ReadLine();

            //check that name of assessment is entered
            if (string.IsNullOrWhiteSpace(n))
            {
                Console.WriteLine("Invalid input. Assessment name can't be empty");
                Pause("Press any key to continue...");
                return;
            }
            
            //Ask user for due date
            Console.WriteLine("Enter due date (dd/MM/yyyy): ");
            string? dueDateInput = Console.ReadLine();

            //Validate the date against current date
            if (DateTime.TryParseExact(dueDateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dueDate))
            {
                       
                //Ask user for result or N/A if not marked
                Console.WriteLine("Enter assessment result (or N/A if not marked yet): ");
                string? r = Console.ReadLine();

                //to check for user input
                if (string.IsNullOrWhiteSpace(r))
                {
                    Console.WriteLine("Invalid input. Result cannot be empty");
                    Pause("Press any key to continue...");
                    return;
                }

                //Check status of assessment (Due/Overdue)
                string status = (dueDate.Date < DateTime.Today) ? "Overdue" : "Due";

                //Format as name|dueDate|result|status
                string newAssessment = $"{n}|{dueDate:dd/MM/yyyy}|{r}|{status}";

                results.Add(newAssessment);
                WriteResultsToFile(results, filePath);

                Console.WriteLine("Assessment added successfully");
                Pause("Press any key to continue...");


            }
            
        }   //end AddAssessment


        //DeleteAssessment method
        static void DeleteAssessment(List<string> results, string filePath)
        {
            //if the list is empty
            if (results.Count == 0)
            {
                Console.WriteLine("No results to delete.");
                Pause("Press any key to continue...");
                return;
            }
            //display the results list
            DisplayResults(results);
            Console.Write("Enter the number of the Assessment you would like to delete: ");
            string? inputDelete = Console.ReadLine();
            if (int.TryParse(inputDelete, out int resultNumber) && resultNumber >= 1 && resultNumber <= results.Count)
            {
                //remove the result from the list
                results.RemoveAt(resultNumber - 1);
                //write the updated list back to the file
                WriteResultsToFile(results, filePath);
                Console.WriteLine("Assessment deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid assessment number");
            }
            Pause("Press any key to continue...");
        }   //end delete contact

        //ViewassessmentList method
        static void ViewAssessmentList(List<string> results)
        {
            Console.WriteLine("Result List");
            DisplayResults(results);
            Pause("Press any key to continue...");
        }   // end ViewAssessmentList method

        //Pause method
        static void Pause(string message)   //pause method to display message and wait for user input
        {
            Console.WriteLine(message);
            Console.ReadKey();
        
        }   //end pause 



    }//end class
}//end namespace
