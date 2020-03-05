using easyar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTarget : ImageTargetController
{
    public enum State { Lost, Found}
    public State currentState;

    [System.Serializable]
    public class SceneObjectAR
    {
        public MeshRenderer mesh;
        public Transform rootTransform;

        public SceneObjectAR(Transform root)
        {
            rootTransform = root;
            mesh = rootTransform.GetComponentInChildren<MeshRenderer>();
        }

        public void CheckIfBusy(Texture2D texture, Vector3[] positions)
        {
            print("checkPositionsWith " + rootTransform.position);

            Vector3[] positionsPrepared = new Vector3[positions.Length];
            Color[] colors = new Color[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                positionsPrepared[i] = Camera.main.WorldToScreenPoint(positions[i]);
                colors[i] = texture.GetPixel((int)positionsPrepared[i].x, (int)positionsPrepared[i].y);
            }

            Color colorToSet;
            if (IsSpotBusyByAwerage(colors, out colorToSet))
            {
                if (mesh != null)
                {
                    mesh.enabled = true;
                    mesh.material.color = colorToSet;
                }
            }
        }

        public void Disable()
        {
            if (mesh != null)
                mesh.enabled = false;
        }

        [Header("Manage color detection here")]
        public Vector3 colorCeiling = new Vector3(0.7f, 0.7f, 0.7f);

        #region different methods for colo detections
        bool IsSpotBusy(Color color)
        {
            if (color.r < colorCeiling.x || color.g < colorCeiling.y || color.b < colorCeiling.z)
                return true;
            else return false;
        }

        bool IsSpotBusy(Color[] colors)
        {
            float highestR = 0;
            float highestG = 0;
            float highestB = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                if (highestR < colors[i].r) highestR = colors[i].r;
                if (highestG < colors[i].g) highestG = colors[i].g;
                if (highestB < colors[i].b) highestB = colors[i].b;
            }
            print("Amounts of checks: "+ colors.Length +" |||highestRed" +highestR+ "| highestGreen " + highestG + " | highestBlue "+highestB);
            if (highestR < colorCeiling.x || highestG < colorCeiling.y || highestB < colorCeiling.z)
                return true;
            else return false;
        }

        bool IsSpotBusyByAwerage(Color[] colors)
        {
            float awerageR = 0;
            float awerageG = 0;
            float awerageB = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                awerageR += colors[i].r;
                awerageG += colors[i].g;
                awerageB += colors[i].b;
            }

            awerageR = awerageR / colors.Length;
            awerageG = awerageG / colors.Length;
            awerageB = awerageB / colors.Length;
            print("Amounts of colors: " + colors.Length + " |||awerageR" + awerageR + "| awerageG " + awerageG + " | awerageB " + awerageB);
            if (awerageR < colorCeiling.x || awerageG < colorCeiling.y || awerageB < colorCeiling.z)
                return true;
            else return false;
        }

        bool IsSpotBusyByAwerage(Color[] colors, out Color brightestColor)
        {
            brightestColor = new Color(1,1,1);

            float awerageR = 0;
            float awerageG = 0;
            float awerageB = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                awerageR += colors[i].r;
                awerageG += colors[i].g;
                awerageB += colors[i].b;
            }

            awerageR = awerageR / colors.Length;
            awerageG = awerageG / colors.Length;
            awerageB = awerageB / colors.Length;
            print("Amounts of colors: " + colors.Length + " |||awerageR" + awerageR + "| awerageG " + awerageG + " | awerageB " + awerageB);
            if (awerageR < colorCeiling.x || awerageG < colorCeiling.y || awerageB < colorCeiling.z)
            {
                brightestColor = new Color(awerageR, awerageG, awerageB);
                return true;
            } 
            else return false;
        }
        #endregion region
    }
}
