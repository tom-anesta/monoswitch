// Animation.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace monoswitchExample
{
    class Animation
    {
        #region members

            #region public

            #endregion

            #region protected

                // Width of a given frame
                protected int m_frameWidth;
                // Height of a given frame
                protected int m_frameHeight;
                // The state of the Animation
                protected bool m_active;
                // Determines if the animation will keep playing or deactivate after one run
                protected bool m_looping;
                // Width of a given frame
                protected Vector2 m_position;
                // The image representing the collection of images used for animation
                Texture2D m_spriteStrip;
                // The scale used to display the sprite strip
                float m_scale;
                // The time since we last updated the frame
                int m_elapsedTime;
                // The time we display a frame until the next one
                int m_frameTime;
                // The number of frames that the animation contains
                int m_frameCount;
                // The index of the current frame we are displaying
                int m_currentFrame;
                // The color of the frame we will be displaying
                Color m_color;
                // The area of the image strip we want to display
                Rectangle m_sourceRect = new Rectangle();
                // The area where we want to display the image strip in the game
                Rectangle m_destinationRect = new Rectangle();


            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public int frameHeight
                {
                    get
                    {
                        return this.m_frameHeight;
                    }
                    set
                    {
                        return;
                    }
                }

                public int frameWidth
                {
                    get
                    {
                        return this.m_frameWidth;
                    }
                    set
                    {
                        return;
                    }
                }

                public Vector2 position
                {
                    get
                    {
                        return this.m_position;
                    }
                    set
                    {
                        this.m_position = value;
                    }
                }

                public Texture2D spriteStrip
                {
                    get
                    {
                        return this.m_spriteStrip;
                    }
                }

                public int frameCount
                {
                    get
                    {
                        return this.m_frameCount;
                    }
                }

                public int frameTime
                {
                    get
                    {
                        return this.m_frameTime;
                    }
                }

                public Color color
                {
                    get
                    {
                        return this.m_color;
                    }
                }

                public float scale
                {
                    get
                    {
                        return this.m_scale;
                    }
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region methods

            #region public

                public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frametime, Color color, float scale, bool looping)
                {
                    // Keep a local copy of the values passed in
                    this.m_color = color;
                    this.m_frameWidth = frameWidth;
                    this.m_frameHeight = frameHeight;
                    this.m_frameCount = frameCount;
                    this.m_frameTime = frametime;
                    this.m_scale = scale;

                    this.m_looping = looping;
                    this.m_position = position;
                    this.m_spriteStrip = texture;

                    // Set the time to zero
                    this.m_elapsedTime = 0;
                    this.m_currentFrame = 0;

                    // Set the Animation to active by default
                    this.m_active = true;
                }

                public void Update(GameTime gameTime)
                {
                    // Do not update the game if we are not active
                    if (this.m_active == false)
                        return;

                    // Update the elapsed time
                    this.m_elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                    // If the elapsed time is larger than the frame time
                    // we need to switch frames
                    if (this.m_elapsedTime > this.m_frameTime)
                    {
                        // Move to the next frame
                        this.m_currentFrame++;
                        // If the currentFrame is equal to frameCount reset currentFrame to zero
                        if (this.m_currentFrame == this.m_frameCount)
                        {
                            this.m_currentFrame = 0;
                            // If we are not looping deactivate the animation
                            if (this.m_looping == false)
                            {
                                Console.WriteLine("becoming inactive");
                                this.m_active = false;
                            }
                        }
                        // Reset the elapsed time to zero
                        this.m_elapsedTime = 0;
                    }

                    // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
                    this.m_sourceRect = new Rectangle(this.m_currentFrame * this.m_frameWidth, 0, this.m_frameWidth, this.m_frameHeight);
                    // decide where it goes
                    this.m_destinationRect = new Rectangle((int)this.m_position.X - (int)(this.m_frameWidth * m_scale) / 2,
                    (int)this.m_position.Y - (int)(this.m_frameHeight * m_scale) / 2,
                    (int)(this.m_frameWidth * this.m_scale),
                    (int)(this.m_frameHeight * this.m_scale));
                }

                public void Draw(SpriteBatch spriteBatch, float r_value = 0f)
                {
                    // Only draw the animation when we are active
                    if (this.m_active)
                    {
                        Console.WriteLine(this.m_scale);
                        spriteBatch.Draw(this.m_spriteStrip, this.m_position, this.m_sourceRect, this.m_color, -r_value, new Vector2(this.frameWidth / 2, this.frameHeight / 2), this.m_scale, SpriteEffects.None, 0f);
                        Console.WriteLine("drawing animation: " + " x= " + this.m_position.X + ", y= " + this.m_position.Y);
                    }
                    else
                    {
                        Console.WriteLine("not active");
                    }
                }

                public void fastForward()//change the frame position
                {
                    return;
                }



            #endregion

            #region protected

            #endregion

            #region private

            #endregion

       #endregion

    }
}
