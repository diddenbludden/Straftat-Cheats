using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace strafthot
{
    public class Utils
    {
        private static GUIStyle _style = null;
        private static Vector3[] _boxExtents = new Vector3[8];
        private static Vector3 _boxExtent = new Vector3();
        private static Material _drawMaterial = null;
        private static Material GetDrawMat()
        {
            if (_drawMaterial != null)
                return _drawMaterial;

            _drawMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
            {
                hideFlags = (HideFlags)61
            };
            _drawMaterial.SetInt("_SrcBlend", 5);
            _drawMaterial.SetInt("_DstBlend", 10);
            _drawMaterial.SetInt("_Cull", 0);
            _drawMaterial.SetInt("_ZWrite", 0);

            return _drawMaterial;
        }

        public static void DrawText(Vector2 position, string text, Color color, int fontSize, bool center = false)
        {
            if (_style == null)
            {
                _style = new GUIStyle();
                _style.fontSize = 14;
            }

            Vector2 drawPos = position;
            if (center)
            {
                Vector2 textSize = _style.CalcSize(new GUIContent(text));
                drawPos.x -= textSize.x / 2;
                drawPos.y -= textSize.y / 2;
            }

            _style.fontSize = fontSize;
            _style.normal.textColor = Color.black;
            GUI.Label(new Rect(drawPos.x - 1, drawPos.y + 1, 1000, 1000), text, _style);

            _style.normal.textColor = color;
            GUI.Label(new Rect(drawPos.x, drawPos.y, 1000, 1000), text, _style);
        }
        public static void SetupExtentsBounds(Bounds b)
        {
            _boxExtent.x = b.center.x + b.size.x / 2f;
            _boxExtent.y = b.center.y + b.size.y / 2f;
            _boxExtent.z = b.center.z + b.size.z / 2f;
            _boxExtents[0] = (_boxExtent);

            _boxExtent.x = b.center.x + b.size.x / 2f;
            _boxExtent.y = b.center.y + b.size.y / 2f;
            _boxExtent.z = b.center.z - b.size.z / 2f;
            _boxExtents[1] = (_boxExtent);

            _boxExtent.x = b.center.x + b.size.x / 2f;
            _boxExtent.y = b.center.y - b.size.y / 2f;
            _boxExtent.z = b.center.z + b.size.z / 2f;
            _boxExtents[2] = (_boxExtent);

            _boxExtent.x = b.center.x + b.size.x / 2f;
            _boxExtent.y = b.center.y - b.size.y / 2f;
            _boxExtent.z = b.center.z - b.size.z / 2f;
            _boxExtents[3] = (_boxExtent);

            _boxExtent.x = b.center.x - b.size.x / 2f;
            _boxExtent.y = b.center.y + b.size.y / 2f;
            _boxExtent.z = b.center.z + b.size.z / 2f;
            _boxExtents[4] = (_boxExtent);

            _boxExtent.x = b.center.x - b.size.x / 2f;
            _boxExtent.y = b.center.y + b.size.y / 2f;
            _boxExtent.z = b.center.z - b.size.z / 2f;
            _boxExtents[5] = (_boxExtent);

            _boxExtent.x = b.center.x - b.size.x / 2f;
            _boxExtent.y = b.center.y - b.size.y / 2f;
            _boxExtent.z = b.center.z + b.size.z / 2f;
            _boxExtents[6] = (_boxExtent);

            _boxExtent.x = b.center.x - b.size.x / 2f;
            _boxExtent.y = b.center.y - b.size.y / 2f;
            _boxExtent.z = b.center.z - b.size.z / 2f;
            _boxExtents[7] = (_boxExtent);
        }

        public static void Draw3DBox(Camera camera, Color color)
        {
            GL.PushMatrix();
            GL.LoadProjectionMatrix(camera.projectionMatrix);
            GL.modelview = camera.worldToCameraMatrix;
            GL.Begin(1);
            GetDrawMat().SetPass(0);
            GL.Color(color);
            GL.Vertex(_boxExtents[0]);
            GL.Vertex(_boxExtents[1]);
            GL.Vertex(_boxExtents[1]);
            GL.Vertex(_boxExtents[5]);
            GL.Vertex(_boxExtents[5]);
            GL.Vertex(_boxExtents[4]);
            GL.Vertex(_boxExtents[4]);
            GL.Vertex(_boxExtents[0]);
            GL.Vertex(_boxExtents[2]);
            GL.Vertex(_boxExtents[3]);
            GL.Vertex(_boxExtents[3]);
            GL.Vertex(_boxExtents[7]);
            GL.Vertex(_boxExtents[7]);
            GL.Vertex(_boxExtents[6]);
            GL.Vertex(_boxExtents[6]);
            GL.Vertex(_boxExtents[2]);
            GL.Vertex(_boxExtents[2]);
            GL.Vertex(_boxExtents[0]);
            GL.Vertex(_boxExtents[3]);
            GL.Vertex(_boxExtents[1]);
            GL.Vertex(_boxExtents[7]);
            GL.Vertex(_boxExtents[5]);
            GL.Vertex(_boxExtents[6]);
            GL.Vertex(_boxExtents[4]);

            GL.End();
            GL.PopMatrix();
        }

        public static void Draw2DBox(Rect rect, Color color)
        {
            Material mat = GetDrawMat();
            mat.SetPass(0);

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);

            GL.Begin(GL.LINES);
            GL.Color(color);
            Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMax, 0);
            Vector3 bottomRight = new Vector3(rect.xMax, rect.yMax, 0);
            Vector3 topRight = new Vector3(rect.xMax, rect.yMin, 0);
            Vector3 topLeft = new Vector3(rect.xMin, rect.yMin, 0);
            GL.Vertex(bottomLeft);
            GL.Vertex(bottomRight);

            GL.Vertex(bottomRight);
            GL.Vertex(topRight);

            GL.Vertex(topRight);
            GL.Vertex(topLeft);

            GL.Vertex(topLeft);
            GL.Vertex(bottomLeft);

            GL.End();
            GL.PopMatrix();
        }




        public static Color DoubleColorLerp(float percent, Color full, Color middle, Color empty)
        {
            if (percent < 0.5f)
                return Color.Lerp(empty, middle, percent * 2f);
            return Color.Lerp(middle, full, (percent - 0.5f) * 2f);
        }
        public static Transform RecursiveFind(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform found = RecursiveFind(child, name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}
