using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace monoswitchExample.game
{
    class star
    {

        #region members

            #region public

            #endregion

            #region protected
                
                // The position of the star relative to the top left corner of thescreen
                protected Vector2 m_position;
                protected Vector2 m_altPosition;
                //the alternative position for when the star is outside the bounds
                // Animation representing the star
                protected Animation m_starAnimation;
                protected Animation m_altStarAnimation;//the alternative animation for when the star is outside the bounds
                // The state of the star
                protected bool m_active;
                // The hit points of the star, if this goes to zero (player hits star) the star is collected
                protected int m_health;
                // The amount of damage the star inflicts on the player ship
                //protected int m_damage;
                // The amount of score the star will give to the player
                protected int m_value;
                // The speed at which the star moves
                //float m_moveSpeed;
                protected int m_radius;
                protected Viewport m_port;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public bool active
                {
                    get
                    {
                        return this.m_active;
                    }
                    set
                    {
                        this.m_active = value;
                    }
                }

                public bool iOOB
                {
                    get
                    {
                        return (this.m_altStarAnimation != null);
                    }
                }

                public int value
                {
                    get
                    {
                        return this.m_value;
                    }
                }

                public int health
                {
                    get
                    {
                        return this.m_health;
                    }
                    set
                    {
                        this.m_health = value;
                    }
                }

                public Vector2 position
                {
                    get
                    {
                        return this.m_position;
                    }
                }


                public Rectangle colRect
                {
                    get
                    {
                        return new Rectangle();
                    }
                }

                public Rectangle altColRect
                {
                    get
                    {
                        return new Rectangle();
                    }
                }

                public float radius
                {
                    get
                    {
                        return this.m_radius;
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

                // Get the width of the enemy ship
                public int Width
                {
                    get { return this.m_starAnimation.frameWidth; }
                }

                // Get the height of the enemy ship
                public int Height
                {
                    get { return this.m_starAnimation.frameHeight; }
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                public void Initialize(Animation animation, Vector2 position, Viewport port)
                {
                    // Set the position of the enemy
                    this.m_position = position;
                    this.m_port = port;
                    this.m_altPosition = Vector2.Zero;//non-nullable
                    // Load the enemy ship texture
                    this.m_starAnimation = animation;
                    this.m_altStarAnimation = null;
                    Vector2 altpos;
                    if (this.isOutOfBounds(this.m_port, out altpos))
                    {
                        this.m_altPosition = altpos;
                        this.m_altStarAnimation = new Animation();
                        this.m_altStarAnimation.Initialize(this.m_starAnimation.spriteStrip, this.m_altPosition, this.m_starAnimation.frameWidth, this.m_starAnimation.frameHeight, this.m_starAnimation.frameCount, this.m_starAnimation.frameTime, this.m_starAnimation.color, this.m_starAnimation.scale, true);
                    }

                    // We initialize the enemy to be active so it will be update in the game
                    this.m_active = true;
                    // Set the health of the enemy
                    this.m_health = 10;
                    // Set the amount of damage the enemy can do
                    //this.m_damage = 10;
                    // Set how fast the enemy moves
                    //this.m_moveSpeed = 6f;
                    // Set the score value of the enemy
                    this.m_value = 100;
                    this.m_radius = (int)(Math.Sqrt(Math.Pow(this.Height, 2) + Math.Pow(this.Width, 2)) / 2);


                }

                public void Update(GameTime gameTime)
                {
                    // Update the position of the Animation
                    this.m_starAnimation.position = this.m_position;
                    // Update Animation
                    this.m_starAnimation.Update(gameTime);
                    if (this.iOOB)
                    {
                        this.m_altStarAnimation.Update(gameTime);
                    }
                }

                public void Draw(SpriteBatch spriteBatch)
                {
                    // Draw the animation
                    this.m_starAnimation.Draw(spriteBatch);
                    if (this.iOOB)
                    {
                        this.m_altStarAnimation.Draw(spriteBatch);
                    }
                }

            #endregion

            #region protected

                protected bool isOutOfBounds(Viewport port, out Vector2 outVect)
                {//this function is a little bit clunky.  we could be looking for ways to improve this if we get more time
                    float bound = (float)Math.Sqrt(Math.Pow((double)this.Height, 2) + Math.Pow((double)this.Width, 2));
                    float greater = Math.Max(this.Height / 2, this.Width / 2);
                    float xloc = 0f;
                    float yloc = 0f;
                    bool isout = false;
                    if ((this.m_position.X + greater + bound) > (port.X + port.TitleSafeArea.Width) || (this.m_position.X + greater - bound) < port.X)
                    {//then out
                        if (this.m_position.X > port.X + port.TitleSafeArea.Width / 2)
                        {
                            xloc = this.m_position.X - port.TitleSafeArea.Width;
                        }
                        else
                        {
                            xloc = this.m_position.X + port.TitleSafeArea.Width;
                        }
                        isout = true;
                    }
                    else
                    {
                        xloc = this.m_position.X;
                    }
                    if ((this.m_position.Y + greater - bound) < port.Y || (this.m_position.Y + greater + bound) > (port.Y + port.TitleSafeArea.Height))
                    {
                        if (this.m_position.Y < port.Y + port.TitleSafeArea.Height / 2)
                        {
                            yloc = this.m_position.Y + port.TitleSafeArea.Height;
                        }
                        else
                        {
                            yloc = this.m_position.Y - port.TitleSafeArea.Height;
                        }
                        isout = true;
                    }
                    else
                    {
                        yloc = this.m_position.Y;
                    }
                    if (isout)
                    {
                        outVect = new Vector2(xloc, yloc);
                    }
                    else
                    {
                        outVect = Vector2.Zero;
                    }
                    return isout;
                }        

            #endregion

            #region private

            #endregion

        #endregion

    }
}
