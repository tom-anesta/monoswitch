// ParallaxingBackground.cs
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace monoswitchExample.game
{
    class parallaxingbackground
    {

        #region members

            #region public

            #endregion

            #region protected

                // The image representing the parallaxing background
                protected Texture2D m_texture;

                // An array of positions of the parallaxing background
                protected Vector2[] m_positions;

                // The speed which the background is moving
                protected int m_speed;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

                #region public

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

                    public void Initialize(ContentManager content, String texturePath, int screenWidth, int speed)
                    {

                        // Load the background texture we will be using
                        this.m_texture = content.Load<Texture2D>(texturePath);

                        // Set the speed of the background
                        this.m_speed = speed;

                        // If we divide the screen with the texture width then we can determine the number of tiles need.
                        // We add 1 to it so that we won't have a gap in the tiling
                        this.m_positions = new Vector2[screenWidth / this.m_texture.Width + 1];

                        // Set the initial positions of the parallaxing background
                        for (int i = 0; i < this.m_positions.Length; i++)
                        {
                            // We need the tiles to be side by side to create a tiling effect
                            this.m_positions[i] = new Vector2(i * this.m_texture.Width, 0);
                        }

                    }

                    public void Update()
                    {
                        // Update the positions of the background
                        for (int i = 0; i < this.m_positions.Length; i++)
                        {
                            // Update the position of the screen by adding the speed
                            this.m_positions[i].X += this.m_speed;
                            // If the speed has the background moving to the left
                            if (this.m_speed <= 0)
                            {
                                // Check the texture is out of view then put that texture at the end of the screen
                                if (this.m_positions[i].X <= -this.m_texture.Width)
                                {
                                    this.m_positions[i].X = this.m_texture.Width * (this.m_positions.Length - 1);
                                }
                            }

                            // If the speed has the background moving to the right
                            else
                            {
                                // Check if the texture is out of view then position it to the start of the screen
                                if (this.m_positions[i].X >= this.m_texture.Width * (this.m_positions.Length - 1))
                                {
                                    this.m_positions[i].X = -this.m_texture.Width;
                                }
                            }
                        }
                    }

                    public void Draw(SpriteBatch spriteBatch)
                    {
                        for (int i = 0; i < this.m_positions.Length; i++)
                        {
                            spriteBatch.Draw(this.m_texture, this.m_positions[i], Color.White);
                        }
                    }

                #endregion

                #region protected

                #endregion

                #region private

                #endregion

        #endregion

    }
}
