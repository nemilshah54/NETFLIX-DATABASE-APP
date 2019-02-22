/*Program.cs*/

//
// Nemil R Shah
// Project Name :  Netflix database application
//

using System;
using System.Data;
using System.Data.SqlClient;

namespace program
{

  class Program
  {
    //
    // Connection info for ChicagoCrimes database in Azure SQL:
    //
    static string connectionInfo = String.Format(@"
Server=tcp:jhummel2.database.windows.net,1433;Initial Catalog=Netflix;
Persist Security Info=False;User ID=student;Password=cs341!uic;
MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;
Connection Timeout=30;
");
    
    // FUnction to Compute the top ten Movies.------------------3
    static void  computeTopTen()
    {
        SqlConnection db = null;
        DataSet ds = new DataSet();
        
        try
        {
            //Open the connection in data base server.
            db = new SqlConnection(connectionInfo);
            db.Open();
            
            //Query Builder.
            string sql = string.Format(@"SELECT TOP 10 Movies.MovieID, Count(Movies.MovieID) AS TotalRev, AVG(CONVERT(float, Rating)) AS AvgRate, MovieName
                                         FROM Movies
                                         INNER JOIN Reviews ON Movies.MovieID = Reviews.MovieID
                                        GROUP BY Movies.MovieID, MovieName
                                        ORDER BY AvgRate DESC");

            
            // Retrieving multiple value from database.
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.CommandText = sql;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            adapter.Fill(ds);
            
            // No more connection needed so closing.
            db.Close();
            
            var rows = ds.Tables["TABLE"].Rows;  

            // Outputting top ten movies.
           Console.WriteLine("Rank\tMovieID\tNumReviews\tAvgRating\tMovieName");
            
          int rank=1;
          foreach (DataRow row in rows)
          {  
              System.Console.WriteLine("{0}\t{1}\t{2}\t\t{3:0.00000}\t\t'{4}'",  rank, row["MovieID"], row["TotalRev"], row["AvgRate"], row["MovieName"]);
              rank++;
          }
       
        }
        
        
      catch (Exception ex)
      {
        System.Console.WriteLine();
        System.Console.WriteLine("**Error: {0}", ex.Message);
        System.Console.WriteLine();
      }
      finally
      {
				// make sure we close connection no matter what happens:
        if (db != null && db.State != ConnectionState.Closed)
          db.Close();
      } 
        
    }   
   
    // Function to describe the userinfo...  
    static void userInfo()
    {
        SqlConnection db = null;
        DataSet ds = new DataSet();   // For retreiving mutiple values.
        
        try
        {
            Console.Write("Enter user id or name >> ");
          
            string text = System.Console.ReadLine();
            text = text.Replace("'","''");
            int id;
             string sql = null;
            
            // Open the Connection to server.
            db = new SqlConnection(connectionInfo);
            db.Open();
            Console.Write("\n\n");
            
            if(System.Int32.TryParse(text, out id))
            {
                // Query Builder.
                 sql = string.Format(@"SELECT UserName, Users.UserID, Occupation, Count(Rating) AS NumStars, AVG(CONVERT(float, Rating)) AS NumStar
                                       FROM Users
                                        FULL OUTER JOIN Reviews ON  Users.UserID= Reviews.UserID
                                        WHERE Users.UserID = '{0}'
                                        GROUP BY Users.UserID, UserName, Occupation, Rating
                                    ",id);            
            }
            
            else
            {
                  // Query Builder.
                   sql = string.Format(@"SELECT UserName, Users.UserID, Occupation, Count(Rating) AS NumStars, AVG(CONVERT(float, Rating)) AS NumStar
                                                      FROM Users
                                                     FULL OUTER JOIN Reviews ON  Users.UserID= Reviews.UserID
                                                    WHERE Users.UserName =  '{0}'
                                                    GROUP BY Users.UserID, UserName, Occupation, Rating
                                                   ",text);          
                   
            }
            
            
            
           // Retrieving multiple value from database.
           SqlCommand cmd = new SqlCommand();
           cmd.Connection = db;
           cmd.CommandText = sql;
           SqlDataAdapter adapter = new SqlDataAdapter(cmd);
           adapter.Fill(ds);
           db.Close();
            
           var rows = ds.Tables["TABLE"].Rows; 
           
           // Variables to retrieve values from database.
           string userName = null;
           double userId = 0.0;
           string occupation=null;
           double[] numStar = new double[5];
           double totalStars;
           double sum;
           double avg;
           int i=0;
           int x = 5;
 
         // Outputting userinfo.   
         if ( rows.Count != 0) 
         {
             foreach (DataRow row in rows)
             {
                 if(row["NumStar"] == DBNull.Value)
                 {
                     x=1;
                     userName = System.Convert.ToString( row["UserName"]);
                     userId = System.Convert.ToDouble( row["UserID"]);
                     occupation = System.Convert.ToString( row["Occupation"]);
                 }
                 
                 else
                 {
                      x=0;
                     userName = System.Convert.ToString( row["UserName"]);
                     userId = System.Convert.ToDouble( row["UserID"]);
                     occupation = System.Convert.ToString( row["Occupation"]);
                     double temp = System.Convert.ToDouble(row["NumStars"]);
                     int number = System.Convert.ToInt32(row["NumStar"]);
                     numStar[number-1] = temp;
                     i++;
                 }
                 
             }
         }
             
          
        if ( x == 0)
        {
            
          sum = numStar[0]*1 + numStar[1]*2 + numStar[2]*3 + numStar[3]*4 + numStar[4]*5;
          totalStars = numStar[0] + numStar[1] + numStar[2] + numStar[3] + numStar[4];
          avg =  sum/totalStars;
          
          System.Console.WriteLine("{0}\nUser id: {1}\nOccupation: {2}", userName , userId , occupation); 
          System.Console.WriteLine("Avg rating : {0:0.00000}", avg);
          System.Console.WriteLine("Num reviews: {0}", totalStars);
          System.Console.WriteLine(" 1 Star : {0}", numStar[0]);
          System.Console.WriteLine(" 2 Stars: {0}",numStar[1]);
          System.Console.WriteLine(" 3 Stars: {0}", numStar[2]);
          System.Console.WriteLine(" 4 Stars: {0}", numStar[3]);
          System.Console.WriteLine(" 5 Stars: {0}", numStar[4]); 
        }
             
        else if (x==1)
        {
                      System.Console.WriteLine("{0}\nUser id: {1}\nOccupation: {2}", userName , userId , occupation); 
                      System.Console.WriteLine("Avg rating: N/A");
                      System.Console.WriteLine("Num reviews: 0"); 
        }
   
         else
         {
             System.Console.WriteLine("** User not found...");
         }

        }
 
      catch (Exception ex)
      {
        System.Console.WriteLine();
        System.Console.WriteLine("**Error: {0}", ex.Message);
        System.Console.WriteLine();
      }
      finally
      {
				// make sure we close connection no matter what happens:
        if (db != null && db.State != ConnectionState.Closed)
          db.Close();
      }
        
        
    }

    // Function to describe the movie info.
    static void movieInfo()
    {
      SqlConnection db = null;
      DataSet ds = new DataSet();

      try
      {
          
        System.Console.Write("Enter a movie id or part of the movie name >>");
  
        string sql = null;
        string text  =  System.Console.ReadLine();
        string  code    =  System.Convert.ToString(text);
        int id;
        text = text.Replace("'","''");

        //Open the connection in data base server.
        db = new SqlConnection(connectionInfo);
        db.Open();
       Console.Write("\n\n");
          
        if(System.Int32.TryParse(text,  out id))
        {
            //Query Builder.
            sql = string.Format(@"  SELECT Movies.MovieID, MovieName, MovieYear,  Count(Movies.MovieID) AS TotalRev, AVG(CONVERT(float,Rating))  AS  AvgRate
                                    FROM Movies
                                    LEFT JOIN Reviews ON Movies.MovieID = Reviews.MovieID
                                    WHERE Movies.MovieID = {0}
                                    GROUP BY Movies.MovieID, MovieName, MovieYear
                                    ORDER BY MovieName", id);
        }
        else
        {
            //Query Builder.
           sql = string.Format(@"SELECT Movies.MovieID, MovieName, MovieYear, Count(Movies.MovieID) AS TotalRev, AVG(CONVERT(float,Rating))  AS  AvgRate 
                                    FROM Movies
                                    LEFT JOIN Reviews ON Movies.MovieID = Reviews.MovieID
                                    WHERE Movies.MovieName Like '%{0}%'
                                    GROUP BY Movies.MovieID, MovieName, MovieYear
                                    ORDER BY MovieName", text);     
     
        }  
          
          // Retrieving multiple value from database.
          SqlCommand cmd = new SqlCommand();
          cmd.Connection = db;
          cmd.CommandText = sql;
          SqlDataAdapter adapter = new SqlDataAdapter(cmd);
          adapter.Fill(ds);
          
           // No more connection needed so closing.
           db.Close();

           var rows = ds.Tables["TABLE"].Rows;
         //  System.Console.WriteLine(rows.Count); 
         
           // Outputting the userinfo.
          if ( rows.Count != 0)
          {
           foreach (DataRow row in rows) 
           {

             if ( row["AvgRate"] !=  DBNull.Value )
               {
                  // System.Console.WriteLine("** yesss");
                    System.Console.WriteLine("{0}\n'{1}'\nYear: {2}\nNum reviews: {3}\nAvg rating: {4:0.00000}",  row["MovieID"],  row["MovieName"], 
                                           row["MovieYear"], row["TotalRev"], row["AvgRate"]);
                  System.Console.WriteLine("\n");
                   
               }
               else
               {
                  // System.Console.WriteLine("nooo");
                     System.Console.WriteLine("{0}\n'{1}'\nYear: {2}\nNum reviews: {3}\nAvg rating: {4:0.00000}",  row["MovieID"],  row["MovieName"], 
                                           row["MovieYear"], 0, "N/A");
                  System.Console.WriteLine("\n");
               } 
   
           }
          } 
          else
          {
              System.Console.WriteLine("** Movie not found");
          }     
      }
        
      catch (Exception ex)
      {
        System.Console.WriteLine();
        System.Console.WriteLine("**Error: {0}", ex.Message);
        System.Console.WriteLine();
      }
      finally
      {
				// make sure we close connection no matter what happens:
        if (db != null && db.State != ConnectionState.Closed)
          db.Close();
      }
        
    }


    static string GetUserCommand()
    {
      System.Console.WriteLine();
      System.Console.WriteLine("What would you like?");
      System.Console.WriteLine("m. movie info");
      System.Console.WriteLine("t. top-10 info");
      System.Console.WriteLine("u. user info");
      System.Console.WriteLine("x. exit");
      System.Console.Write(">> ");

      string cmd = System.Console.ReadLine();

      return cmd.ToLower();
    }


    //
    // Main:
    //
    static void Main(string[] args)
    {
      System.Console.WriteLine("** Netflix Database App **");

      string cmd = GetUserCommand();

      while (cmd != "x")
      {
				//
				// This is just an example, you'll want to delete this call to OutputNumMovies:
				//
        if ( cmd == "u")
        {
                System.Console.WriteLine();
                userInfo();

        }
          
       if(cmd == "m")
       {
                System.Console.WriteLine();
				movieInfo();
       }
          
       if ( cmd == "t")
       {
           computeTopTen();
       }
          
          
          
        System.Console.WriteLine();
			//	OutputNumMovies();

        cmd = GetUserCommand();
      }

      System.Console.WriteLine();
      System.Console.WriteLine("** Done **");
      System.Console.WriteLine();
    }

  }//class
}//namespace

