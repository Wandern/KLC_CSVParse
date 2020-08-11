using CsvHelper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

/*
Requirements provided by Elizabeth Schepens 
1) Reads the whova.csv
2) Gets the rows in the.csv file where the total values in the final two columns are greater than 40 and if there's an e-mail address in that row.
3) Creates a new output.csv that contains a header in this format:    Name, Email, Title, Total Minutes
4) Outputs each row in the .csv where the total values in the final two columns are greater than 40 and there's an e-mail address.  

 */

namespace KLC_CSVParse
{
    class Program
    {
        static void Main(string[] args)
        {   //Setup CSV reading variables
            using (var reader = new StreamReader("/whova.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Regular expression to validate the email field in the input csv file. 
                var regexEmail = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                // List to store output csv records in
                var records = new List<OutputType>();
                
                // Read the CSV File
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    // Set the minutes watched fields to zero if it is currently blank.
                    var live = csv.GetField("Live Stream Watch Duration (in minutes)");
                    if (live == "") live = "0";
                    var recorded = csv.GetField("Recorded Video Watch Duration (in minutes)");
                    if (recorded == "") recorded = "0";

                    // Create an output object using the fields from the input file.
                    var record = new OutputType
                    {
                        Name = csv.GetField("Name"),
                        Email = csv.GetField("Email"),
                        Title = csv.GetField("Title"),
                        TotalMinutes = Int32.Parse(live) + Int32.Parse(recorded)
                    };
                    // Validate the email
                    bool validEmail = Regex.IsMatch(record.Email, regexEmail, RegexOptions.IgnoreCase);

                    // If the input record contains a valid email the total minutes watched is greater than 40, add the record to the output list.
                    if (validEmail && record.TotalMinutes > 40)
                    {
                        records.Add(record);
                    }

                }
                // Write the Output list to C:/output.csv
                using (var writer = new StreamWriter("/output.csv"))
                using (var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvOut.WriteRecords(records);
                }



            }
        }

        // Class to store the required data for each ror in output.csv
        public class OutputType
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Title { get; set; }
            [CsvHelper.Configuration.Attributes.Name("Total Minutes")]
            public int TotalMinutes { get; set; }
        }
    }
}
