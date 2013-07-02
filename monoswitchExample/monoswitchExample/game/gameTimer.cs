using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;  

//this class uses code from the ExampleTimer posted to the XBox Forums by user JaceAce01 at http://www.xnawiki.com/index.php?title=Displaying_seconds
//all credit for the lifted portions of code goes to user JaceAce01

namespace monoswitchExample.game
{

    public delegate void timerCompleteHandler(object sender, EventArgs e);

    class gameTimer
    {
        #region members

            #region public

            #endregion

            #region protected

            #endregion

            #region private

                private int startCount;
                private int endCount;

            #endregion

        #endregion

        #region properties

            #region public

                public String displayValue { get; private set; }
                public Boolean isActive { get; private set; }
                public Boolean isComplete { get; private set; }  

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

                public event timerCompleteHandler timerComplete;

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                //constructor
                public gameTimer()
                {  
                    this.isActive = false;  
                    this.isComplete = false;  
                    this.displayValue = "None";  
                    this.startCount = 0;
                    this.endCount = 0;  
                }

                public void set(GameTime gameTime, int seconds)
                {
                    this.startCount = gameTime.TotalGameTime.Seconds;
                    this.endCount = seconds;//this.startCount + seconds;
                    this.isActive = true;
                    this.displayValue = this.endCount.ToString();
                }

                public void Update(GameTime gameTime)
                {
                    if (this.isActive)
                    {
                        if(this.checkTimer(gameTime))
                        {
                            this.isActive = false;
                        }
                    }
                }
                public Boolean checkTimer(GameTime gameTime)
                {
                    if (this.isComplete == false)
                    {
                        if (gameTime.TotalGameTime.Seconds > this.startCount)
                        {
                            this.endCount = this.endCount - (gameTime.TotalGameTime.Seconds - this.startCount);
                            this.startCount = gameTime.TotalGameTime.Seconds;
                            this.displayValue = this.endCount.ToString();
                            if (this.endCount < 0)
                            {
                                this.endCount = 0;
                                this.startCount = 0;
                                this.isComplete = true;
                                this.isActive = false;
                                this.displayValue = "0";
                                if (timerComplete != null)
                                {
                                    //Console.WriteLine("timer complete");
                                    this.timerComplete(this, new EventArgs());
                                }
                            }
                        }
                    }
                    else
                    {
                        this.displayValue = "0";
                    }
                    
                    return this.isComplete;
                }

                public void reset()
                {
                    this.isActive = false;
                    this.isComplete = false;
                    this.displayValue = "None";
                    this.startCount = 0;
                    this.endCount = 0;
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion



    }
}
