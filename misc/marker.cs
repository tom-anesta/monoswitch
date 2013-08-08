using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate.GUI.Content;
using Microsoft.Xna.Framework;
using Ruminate.GUI.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monoswitch.misc
{
    public class marker:WidgetBase<LabelRenderRule>
    {

        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected Vector2 m_vect;
                protected float m_scale;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public float X
                {
                    get
                    {
                        return this.m_vect.X;
                    }
                }

                public float fullX
                {
                    get
                    {
                        return this.m_vect.X * this.m_scale;
                    }
                }

                public float Y
                {
                    get
                    {
                        return this.m_vect.Y;
                    }
                }

                public float fullY
                {
                    get
                    {
                        return this.m_vect.Y * this.m_scale;
                    }
                }

                public float scale
                {
                    get
                    {
                        return this.m_scale;
                    }
                }

                public Texture2D Icon
                {
                    get
                    {
                        return RenderRule.Image;
                    }
                    set
                    {
                        RenderRule.Image = value;
                    }
                }
                

            #endregion

            #region internal

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region methods

            #region public

                public marker(Texture2D img, float xv, float yv, float s)
                {
                    this.Icon = img;
                    this.m_vect = new Vector2(xv, yv);
                    this.m_vect.Normalize();
                    this.m_scale = Math.Abs(s);
                }

                public void moveTo(float x, float y)
                {
                    float xback = this.Icon.Width/2;
                    float yback = this.Icon.Height/2;
                    float xcent = x+this.m_vect.X*this.m_scale;
                    float ycent = y+this.m_vect.Y*this.m_scale;
                    this.AbsoluteArea = new Rectangle( (int)(xcent - xback), (int)(ycent - yback), this.Icon.Width, this.Icon.Height);
                }

            #endregion

            #region internal

            #endregion

            #region protected

                protected override LabelRenderRule BuildRenderRule()
                {
                    return new LabelRenderRule();
                }

                protected override void Attach() { }

                protected override void Update() { }       

            #endregion

            #region private

            #endregion

        #endregion

    }
}
