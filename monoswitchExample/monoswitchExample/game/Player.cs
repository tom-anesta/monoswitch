using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Diagnostics;

namespace monoswitchExample
{

    public enum directions
    {
        right, upright, up, upleft, left, downleft, down, downright, none
    }

    class Player
    {

        #region members

            #region public

                public const float DEFAULT_SPEED = 70.0f;
                public const float DEFAULT_RADIUS = 70.0f;

            #endregion

            #region protected

                // Position of the Player relative to the upper left side of the screen
                protected Vector2 m_position;
                protected Vector2 m_altPosition;//the position outside of the screen
                // State of the player
                protected bool m_active;
                // Amount of hit points that player has
                protected int m_health;
                // Animation representing the player
                protected Animation m_playerAnimation;
                protected Animation m_altPlayerAnimation;//the position outside of the screen
                //what direction is the player facing?
                protected directions m_playerDirection;//what direction is the player going in?
                protected directions m_pendingDirection;//what direction is the player going to be going in next?
                protected float m_rotationAngle;//http://msdn.microsoft.com/en-us/library/bb203869.aspx
                protected float m_speed;//what is the speed the player can go at?
                protected Viewport m_port;
                protected float m_radius;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                // Get the width of the player ship
                public int Width
                {
                    get { return this.m_playerAnimation.frameWidth; }
                }
                // Get the height of the player ship
                public int Height
                {
                    get { return this.m_playerAnimation.frameHeight; }
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

                public bool iOOB
                {
                    get
                    {
                        return (this.m_altPlayerAnimation != null);
                    }
                }

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

                public float speed
                {
                    get
                    {
                        return this.m_speed;
                    }
                }

                public Animation mainAnimation
                {
                    get
                    {
                        return this.m_playerAnimation;
                    }
                }

                public Animation altAnimation
                {
                    get
                    {
                        return this.m_altPlayerAnimation;
                    }
                }

                public float rotation
                {
                    get
                    {
                        return this.m_rotationAngle;
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
                    this.m_speed = Player.DEFAULT_SPEED;
                    this.m_port = port;
                    // Set the starting position of the player around the middle of the screen and to the back
                    this.m_position = position;
                    this.m_altPosition = Vector2.Zero;
                    this.m_playerAnimation = animation;
                    this.m_playerAnimation.position = this.m_position;
                    
                    Vector2 altpos;
                    if (this.isOutOfBounds(this.m_port, out altpos))
                    {
                        this.m_altPosition = altpos;
                        this.m_altPlayerAnimation = new Animation();
                        this.m_altPlayerAnimation.Initialize(this.m_playerAnimation.spriteStrip, this.m_altPosition, this.m_playerAnimation.frameWidth, this.m_playerAnimation.frameHeight, this.m_playerAnimation.frameCount, this.m_playerAnimation.frameTime, this.m_playerAnimation.color, this.m_playerAnimation.scale, true);
                    }
                    // Set the player to be active
                    this.m_active = true;
                    // Set the player health
                    this.m_health = 100;
                    this.m_playerDirection = directions.right;
                    this.m_rotationAngle = 0;
                    this.m_radius = (int) (Math.Sqrt(Math.Pow(this.Height, 2) + Math.Pow(this.Width, 2)) / 2);
                }

                public void Update(GameTime gameTime)
                {
                    this.m_playerAnimation.Update(gameTime);
                    if (this.m_pendingDirection != directions.none)
                    {
                        this.m_playerDirection = this.m_pendingDirection;
                        int switchVal = (int)this.m_playerDirection;
                        float xval = 0f;
                        float yval = 0f;
                        Vector2 offset = Vector2.Zero;
                        Matrix m;
                        switch (switchVal)
                        {
                            case 0:
                                this.m_rotationAngle = 0;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                            case 1:
                                this.m_rotationAngle = MathHelper.Pi * 2 * 0.125f;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                            case 2:
                                this.m_rotationAngle = MathHelper.Pi * 2 * 0.25f;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                            case 3:
                                this.m_rotationAngle = MathHelper.Pi * 2 * 0.375f;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                            case 4:
                                this.m_rotationAngle = MathHelper.Pi * 2 * 0.5f;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                            case 5:
                                this.m_rotationAngle = MathHelper.Pi * 2 * 0.625f;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                            case 6:
                                this.m_rotationAngle = MathHelper.Pi * 2 * 0.75f;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                            case 7:
                                this.m_rotationAngle = MathHelper.Pi * 2 * 0.875f;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                            default:
                                this.m_rotationAngle = 0;
                                m = Matrix.CreateRotationZ(this.m_rotationAngle);
                                xval += m.M11;
                                yval -= m.M12;
                                offset = new Vector2(xval, yval);
                                offset.Normalize();
                                break;
                        }
                        this.m_position += offset * (this.m_speed * gameTime.ElapsedGameTime.Milliseconds / 1000f);
                        //check if we are fully out of bounds to reset
                        if (this.m_position.X > this.m_port.X + this.m_port.TitleSafeArea.Width || this.m_position.X < this.m_port.X || this.m_position.Y > this.m_port.Y + this.m_port.TitleSafeArea.Height || this.m_position.Y < this.m_port.Y)
                        {
                            float locx = this.m_position.X;
                            float locy = this.m_position.Y;
                            if (this.m_position.X > this.m_port.X + this.m_port.TitleSafeArea.Width)
                            {
                                locx -= this.m_port.TitleSafeArea.Width;
                            }
                            else if (this.m_position.X < this.m_port.X)
                            {
                                locx += this.m_port.TitleSafeArea.Width;
                            }
                            if (this.m_position.Y > this.m_port.Y + this.m_port.TitleSafeArea.Height)
                            {
                                locy -= this.m_port.TitleSafeArea.Height;
                            }
                            else if (this.m_position.Y < this.m_port.Y)
                            {
                                locy += this.m_port.TitleSafeArea.Height;
                            }
                            this.m_position = new Vector2(locx, locy);
                            Console.WriteLine("setting location to x: " + this.m_position.X + ", y: " + this.m_position.Y);
                        }
                        //Console.WriteLine("rotation is " + this.m_rotationAngle);
                    }
                    this.m_playerAnimation.position = this.m_position;
                    Vector2 outVect;
                    bool isOutBool = this.isOutOfBounds(this.m_port, out outVect);
                    this.m_altPosition = outVect;
                    if(this.m_altPlayerAnimation == null && isOutBool)
                    {
                        //Console.WriteLine("receiving new animation");
                        //Console.WriteLine("the standard position is " + "x: " + this.m_position.X + ", y: " + this.m_position.Y);
                        //Console.WriteLine("the alternate position is " + " x: " + this.m_altPosition.X + ", y: " + this.m_altPosition.Y);
                        this.m_altPlayerAnimation = new Animation();
                        this.m_altPlayerAnimation.Initialize(this.m_playerAnimation.spriteStrip, this.m_altPosition, this.m_playerAnimation.frameWidth, this.m_playerAnimation.frameHeight, this.m_playerAnimation.frameCount, this.m_playerAnimation.frameTime, this.m_playerAnimation.color, this.m_playerAnimation.scale, true);
                    }
                    else if (isOutBool)
                    {
                        this.m_altPosition = outVect;
                        this.m_altPlayerAnimation.position = this.m_altPosition;
                        this.m_altPlayerAnimation.Update(gameTime);
                        //Console.WriteLine("continuing alt animation at x: " + this.m_altPlayerAnimation.position.X + ", y: " + this.m_altPlayerAnimation.position.Y);
                        //Console.WriteLine("active state is " + this.m_altPlayerAnimation.active);
                    }
                    else
                    {
                        if (this.m_altPlayerAnimation != null)
                        {
                            //Console.WriteLine("leaving alt animation");
                            this.m_altPosition = Vector2.Zero;
                            this.m_altPlayerAnimation = null;
                        }
                    }
                }

                public void Draw(SpriteBatch spriteBatch)
                {
                    this.m_playerAnimation.Draw(spriteBatch, this.m_rotationAngle, 0.2f);
                    if (this.iOOB)
                    {
                        this.m_altPlayerAnimation.Draw(spriteBatch, this.m_rotationAngle, 0.1f);
                    }
                    
                }

                public void HandleInput(KeyboardState k_state, GamePadState g_state, bool g_connected)
                {
                    int hval = 1;
                    int vval = 1;
                    // Otherwise move the player position.
                    Vector2 movement = Vector2.Zero;
                    if (k_state != null)
                    {
                        if (k_state.IsKeyDown(Keys.Left) && !k_state.IsKeyDown(Keys.Right))
                        {
                            hval = 0;
                        }
                        if (k_state.IsKeyDown(Keys.Right) && !k_state.IsKeyDown(Keys.Left))
                        {
                            hval = 2;
                        }
                        if (k_state.IsKeyDown(Keys.Up) && !k_state.IsKeyDown(Keys.Down))
                        {
                            vval = 2;
                        }
                        if (k_state.IsKeyDown(Keys.Down) && !k_state.IsKeyDown(Keys.Up))
                        {
                            vval = 0;
                        }
                    }
                    if (g_state != null && g_connected && k_state == null)
                    {
                        Vector2 thumbstick = g_state.ThumbSticks.Left;
                        movement.X += thumbstick.X;
                        movement.Y += thumbstick.Y;
                        if (movement.Length() > 1)
                        {
                            movement.Normalize();
                        }
                        if(hval == 1)
                        {
                            if(movement.X < 0)
                            {
                                hval = 0;
                            }
                            else if(movement.X > 0)
                            {
                                hval = 2;
                            }
                        }
                        if(vval == 1)
                        {
                            if(movement.Y > 0)
                            {
                                vval = 2;
                            }
                            else if(movement.Y < 0)
                            {
                                vval = 0;
                            }
                        }
                    }
                    else if(k_state == null)
                    {
                        throw new ArgumentNullException("input");
                    }

                    //now handle the coming position command
                    if (hval == 2 && vval == 1)
                    {
                        this.m_pendingDirection = directions.right;
                    }
                    else if (hval == 2 && vval == 2)
                    {
                        this.m_pendingDirection = directions.upright;
                    }
                    else if (hval == 1 && vval == 2)
                    {
                        this.m_pendingDirection = directions.up;
                    }
                    else if (hval == 0 && vval == 2)
                    {
                        this.m_pendingDirection = directions.upleft;
                    }
                    else if (hval == 0 && vval == 1)
                    {
                        this.m_pendingDirection = directions.left;
                    }
                    else if (hval == 0 && vval == 0)
                    {
                        this.m_pendingDirection = directions.downleft;
                    }
                    else if (hval == 1 && vval == 0)
                    {
                        this.m_pendingDirection = directions.down;
                    }
                    else if (hval == 2 && vval == 0)
                    {
                        this.m_pendingDirection = directions.downright;
                    }
                    else
                    {
                        this.m_pendingDirection = directions.none;
                    }
                }

            #endregion

            #region protected

                protected bool isOutOfBounds(Viewport port, out Vector2 outVect)
                {//this function is a little bit clunky.  we could be looking for ways to improve this if we get more time
                    float bound = (float)Math.Sqrt(Math.Pow((double)this.Height, 2) + Math.Pow((double)this.Width, 2));
                    float greater = Math.Max(this.Height/2, this.Width/2);
                    float xloc = 0f;
                    float yloc = 0f;
                    bool isout = false;
                    if ( (this.m_position.X + greater + bound) > (port.X + port.TitleSafeArea.Width) || (this.m_position.X + greater - bound) < port.X)
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
                    if ( (this.m_position.Y + greater - bound) < port.Y || (this.m_position.Y + greater + bound) > (port.Y + port.TitleSafeArea.Height) )
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
