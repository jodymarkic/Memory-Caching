/*
 *  FILENAME        : MainWindow.xaml.cs
 *  PROJECT         : MemoryCaching
 *  PROGRAMMER      : Jody Markic
 *  FIRST VERSION   : 2/21/2018
 *  DESCRIPTION     : This is the code behind for the WPF application MemoryCaching, it hold event handles to UI elements, along with
 *                    a method for measuring retrieval of data from a xml file versus retrieval from a memory cache.
 */

//included namespaces
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;

//project namespace
namespace MemoryCaching
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       private MyCache cache;
       private MyStopwatch stopwatch;
       private string filepath = "../../ListOfUsers.xml";

        //
        //  METHOD: MainWindow (Constructor)
        //  DESCRIPTION: Initializes the window MyCache displays on
        //  PARAMETERS: n/a
        //  RETURNS: n/a
        //
        public MainWindow()
        {
            InitializeComponent();
            //instantiate cache and stopwatch
            cache = new MyCache();
            stopwatch = new MyStopwatch();
            //seed combobox
            LoadComboboxItems();
        }

        //
        //  METHOD: LoadComboboxItems
        //  DESCRIPTION: Seeds a combobox based on ListOfUsers.xml
        //  PARAMETERS:  n/a
        //  RETURNS: void
        //
        private void LoadComboboxItems()
        {
            //retrieve all user names from ListofUsers.xml and store into a combobox.
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filepath);
            XmlNodeList xmlnode =  xmldoc.GetElementsByTagName("UserName");

            foreach (XmlNode node in xmlnode)
            {
                UserNameComboBox.Items.Add(node.InnerText.Trim());
            }
        }

        //
        //  METHOD: RetrieveData_Button_Click
        //  DESCRIPTION: event handle for the Retrieve Data button
        //  PARAMETERS: object sender, RoutedEventArgs e
        //  RETURNS:void
        //
        private void RetrieveData_Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UserNameComboBox.SelectedValue.ToString();

            if (cache.CheckCache("USER_CODE")) //retrieve from cache
            {
                RetrieveData(true);
            }
            else //retrive from xml
            {
                RetrieveData(false);
            }
        }

        //
        //  METHOD: CacheData_Button_Click
        //  DESCRIPTION: event handle for the Cache Data button
        //  PARAMETERS: object sender, RoutedEventArgs e
        //  RETURNS: void
        //
        private void CacheData_Button_Click(object sender, RoutedEventArgs e)
        {
            string usercode = "";
            string username = UserNameComboBox.SelectedValue.ToString();

            //retrieve UserCode from xml.
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filepath);
            XmlNodeList xmlNode = xmldoc.GetElementsByTagName("UserName");
            foreach (XmlNode node in xmlNode)
            {
                if (String.Equals(node.InnerText, username))
                {
                    usercode = node.PreviousSibling.InnerText;
                    break;
                }
            }

            //Store UserCode
            List<String> lstFiles = new List<string>();
            lstFiles.Add(System.IO.Path.Combine(Environment.CurrentDirectory, filepath));

            cache.AddToCache("USER_CODE",usercode, MyCachePriority.Default, lstFiles);
        }

        //
        //  METHOD: ClearCache_Button_Click
        //  DESCRIPTION: event handle for the Clear Cache button
        //  PARAMETERS: object sender, RoutedEventArgs e
        //  RETURNS:
        //
        private void ClearCache_Button_Click(object sender, RoutedEventArgs e)
        {
            cache.RemoveCachedItem("USER_CODE");
        }

        //
        //  METHOD: RetrieveData
        //  DESCRIPTION: This method run retrieval of data from a cache or XML file measures it and displays results to a textbox.
        //  PARAMETERS: bool isCached
        //  RETURNS: void
        //
        private void RetrieveData(bool isCached)
        {
            stopwatch.ResetTotalTicks();
            string userName = UserNameComboBox.SelectedValue.ToString();
            string userCode = "";

            if (isCached) //measure speed from cache
            {
                stopwatch.StartWatch(true);

                for (int i = 0; i < 10000; i++)
                {
                    stopwatch.StartWatch(false);

                    //calculate stuff
                    userCode = (string)cache.GetCachedItem("USER_CODE");
                    
                    stopwatch.StopWatch(false);
                    stopwatch.TimeElapsed(i, false);
                }

                stopwatch.StopWatch(true);
                stopwatch.TimeElapsed(1, true);
                string results = stopwatch.AverageTime(10000, true);

                //write out to the textbox.
                CachedTextbox.Text = "Retrieved User Code: " + userCode + "\r" +
                                     "For User: " + userName + "\r" + results;

            }
            else //measure speed from XML file
            {
                stopwatch.StartWatch(true);

                for (int i = 0; i < 10000; i++)
                {
                    stopwatch.StartWatch(false);

                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(filepath);
                    XmlNodeList xmlNode = xmldoc.GetElementsByTagName("UserName");
                    foreach (XmlNode node in xmlNode)
                    {
                        if (String.Equals(node.InnerText, userName))
                        {
                            userCode = node.PreviousSibling.InnerText;
                            break;
                        }
                    }
                    stopwatch.StopWatch(false);
                    stopwatch.TimeElapsed(i, false);
                }

                stopwatch.StopWatch(true);
                stopwatch.TimeElapsed(1, true);
                string results = stopwatch.AverageTime(10000, false);

                //write out to the textbox.
                UncachedTextbox.Text = "Retrieved User Code: " + userCode + "\r" +
                                     "For User: " + userName + "\r" + results;

            }
        }
    }
}
