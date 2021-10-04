using LibreLancer;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace LibreLancer.Interface
{
    [UiLoadable]
    [MoonSharpUserData]
    public class UiRenderable
    {
        [UiContent]
        public List<DisplayElement> Elements { get; set; } = new List<DisplayElement>();

        public void AddElement(DisplayElement el) => Elements.Add(el);
        public void Draw(UiContext context, RectangleF rectangle)
        {
            foreach(var e in Elements) e.Render(context, rectangle);
        }

        public void DrawWithClip(UiContext context, RectangleF rectangle, RectangleF clip)
        {
            context.Mode3D();
            var clipRectangle = context.PointsToPixels(clip);
            context.RenderContext.ScissorRectangle = clipRectangle;
            context.RenderContext.ScissorEnabled = true;
            Draw(context, rectangle);
            context.Mode3D(); //flush
            context.RenderContext.ScissorEnabled = false;
        }
    }

    public class DisplayElement
    {
        public virtual void Render(UiContext context, RectangleF clientRectangle)
        {
        }
    }
}