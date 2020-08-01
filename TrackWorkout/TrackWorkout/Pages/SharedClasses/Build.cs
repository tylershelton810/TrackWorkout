using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SkiaSharp;
using Xamarin.Forms;
using TrackWorkout.Entitys;

namespace TrackWorkout.Pages.SharedClasses
{    
    public class Build
    {        
        public static Tuple<Grid, double> RoutineGrids(XDocument routineXML)
        {
            double totalWeight = 0;

            Grid ExerciseContentDetail = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                Margin = new Thickness(10, 10, 10, 10),
                BackgroundColor = Color.Transparent,
                ColumnDefinitions = {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = 100 }
                            }
            };

            Label ExerciseHeader = new Label
            {
                Text = "Exercise",
                FontFamily = App.CustomBold,
                FontAttributes = FontAttributes.Bold,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
            };
            Label SetsHeader = new Label
            {
                Text = "# of Sets",
                FontFamily = App.CustomBold,
                FontAttributes = FontAttributes.Bold,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
            };

            int rowNumber = 0;

            foreach (var exercise in routineXML.Element("Routine").Elements("Exercise"))
            {
                int numberOfSets = 0;

                foreach (var set in exercise.Elements("Set"))
                {
                    //Take the Number of reps multiplied by the weight for the set
                    int weightToAdd = (Int32.Parse(set.Element("Weight").Value)) * (Int32.Parse(set.Element("Reps").Value));

                    //Add it to the running total for total weight
                    totalWeight = totalWeight + weightToAdd;

                    //Increment the set #
                    numberOfSets++;
                }

                if (rowNumber == 0)
                {
                    //Insert Headers for exercise and sets
                    ExerciseContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                    ExerciseContentDetail.Children.Add(ExerciseHeader, 0, rowNumber);

                    ExerciseContentDetail.Children.Add(SetsHeader, 1, rowNumber);

                    //Increment row
                    rowNumber++;
                }

                //Create labels to add to grid. MUST DO THIS INSIDE OF THE FOR LOOP
                Label ExerciseName = new Label
                {
                    FontSize = 10,
                    FontFamily = App.CustomRegular,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                };
                Label SetsNumber = new Label
                {
                    FontSize = 10,
                    FontFamily = App.CustomRegular,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                };

                //Set the values that will be show on screen
                SetsNumber.Text = numberOfSets.ToString();
                ExerciseName.Text = exercise.Element("Description").Value;

                ExerciseContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                ExerciseContentDetail.Children.Add(ExerciseName, 0, rowNumber);

                //ExerciseContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                ExerciseContentDetail.Children.Add(SetsNumber, 1, rowNumber);

                rowNumber++;
            }

            return Tuple.Create(ExerciseContentDetail, totalWeight);
        }

        public static Tuple<List<Microcharts.Entry>, float, float> WeightGraph(XDocument WeightXML)
        {
            //Set up the data points for the Weight Graph            
            var weightEntries = new List<Microcharts.Entry>();

            //This is used to keep the graph in a reasonable range
            float minWeightValue = 1000;
            float maxWeightValue = 0;
            int countLoop = 0;
            SKColor colorForWeightGraph = SKColor.Parse(App.PrimaryThemeColor.ToHex().ToString());

            try
            {
                //Loop through every entry of weight
                foreach (XElement node in WeightXML.Element("WeightTracker").Elements("Weight"))
                {
                    string weight;
                    DateTime weighInDate;

                    if (App.userInformationApp[0].WeightType == "Lbs")
                    {
                        //Grab the weight in pounds. This will need to work with Kgs as well.               
                        weight = node.Element("Lbs").Element("Value").Value;
                        weighInDate = DateTime.Parse(node.Element("Lbs").Element("Date").Value);
                    }
                    else
                    {
                        weight = node.Element("Kgs").Element("Value").Value;
                        weighInDate = DateTime.Parse(node.Element("Kgs").Element("Date").Value);
                    }

                    //Alternate the colors for the points at each entry.
                    if (colorForWeightGraph == SKColor.Parse(App.PrimaryThemeColor.ToHex().ToString()))
                    {
                        colorForWeightGraph = SKColor.Parse(App.SecondaryThemeColor.ToHex().ToString());
                    }
                    else
                    {
                        colorForWeightGraph = SKColor.Parse(App.PrimaryThemeColor.ToHex().ToString());
                    }

                    //Create the entry
                    Microcharts.Entry AddToGraph = new Microcharts.Entry(float.Parse(weight))
                    {
                        Color = colorForWeightGraph,
                        Label = weighInDate.ToString("MM-dd"),
                        ValueLabel = weight,
                        TextColor = colorForWeightGraph
                    };

                    //Add the entry to the list to be used in the graph
                    weightEntries.Add(AddToGraph);

                    countLoop++;
                }

                int amountToRemove;
                if (countLoop >= 5)
                {
                    amountToRemove = countLoop - 5;
                    weightEntries.RemoveRange(0, amountToRemove);
                }

                foreach (Microcharts.Entry ent in weightEntries)
                {
                    //Readjust the min and max values
                    if (minWeightValue > float.Parse(ent.ValueLabel))
                    {
                        minWeightValue = float.Parse(ent.ValueLabel);
                    }
                    if (maxWeightValue < float.Parse(ent.ValueLabel))
                    {
                        maxWeightValue = float.Parse(ent.ValueLabel);
                    }
                }

                //Create a little ceiling and floor between the min and max values
                if ((minWeightValue - 5) > 0)
                {
                    minWeightValue = minWeightValue - 5;
                }

                maxWeightValue = maxWeightValue + 5;

                return Tuple.Create(weightEntries, minWeightValue, maxWeightValue);
            }
            catch
            {
                return Tuple.Create(weightEntries, minWeightValue, maxWeightValue);
            }

        }

        public static Grid PersonalRecord(XElement PR, int loopCount, int counter, string Weight)
        {
            if (loopCount < counter)
            {
                Grid PRContent = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(5, 0, 5, 10),
                    ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = GridLength.Star },
                                }
                };

                Label ExerciseName = new Label
                {
                    Text = PR.Element("Exercise").Value,
                    FontSize = 12,
                    FontFamily = App.CustomRegular,
                    HeightRequest = 20,
                    BackgroundColor = Color.FromHex("#f5f5f5"),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Fill,
                    TextColor = App.PrimaryThemeColor
                };

                if (PR.Element("WeightType").Value == "Lbs")
                {
                    Weight = PR.Element("Lbs").Value;
                }
                else
                {
                    Weight = PR.Element("Kgs").Value;
                }

                String PRRecordInsert = Weight + " " + PR.Element("WeightType").Value;
                String RecordDateLabel = "Date Set: ";
                string RecordDateValue = DateTime.Parse(PR.Element("RecordDate").Value).ToString("MM/dd/yyyy");

                Frame PRFrame = new Frame
                {
                    HasShadow = false,
                    Padding = 0,
                };

                Grid DateGrid = new Grid
                {
                    Margin = new Thickness(15, 10, 10, 5),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    BackgroundColor = Color.Transparent,
                    ColumnDefinitions =
                                {
                                    new ColumnDefinition{ Width = GridLength.Auto},
                                    new ColumnDefinition{ Width = GridLength.Auto}
                                }
                };

                Grid PRContentDetail = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = Color.White,
                    ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = GridLength.Star }
                                }
                };

                Label PersonalRecord = new Label
                {
                    Text = PRRecordInsert,
                    FontSize = 10,
                    FontFamily = App.CustomBold,
                    Margin = new Thickness(15, 10, 10, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                };

                Label RecordDate = new Label
                {
                    Text = RecordDateLabel,
                    FontSize = 10,
                    FontFamily = App.CustomBold,

                };
                Label RecordDateValueLabel = new Label
                {
                    Text = RecordDateValue,
                    FontSize = 10,
                    FontFamily = App.CustomRegular
                };

                DateGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                DateGrid.Children.Add(RecordDate, 0, 0);
                DateGrid.Children.Add(RecordDateValueLabel, 1, 0);


                PRFrame.Content = PRContentDetail;

                PRContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                PRContent.Children.Add(ExerciseName, 0, 0);

                PRContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                PRContentDetail.Children.Add(PersonalRecord, 0, 0);

                PRContentDetail.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                PRContentDetail.Children.Add(DateGrid, 0, 1);

                PRContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35, GridUnitType.Auto) });
                PRContent.Children.Add(PRFrame, 0, 1);

                return PRContent;
            }
            else
            {
                Grid noValue = new Grid();

                return noValue;
            }
        }
               
    }
}
