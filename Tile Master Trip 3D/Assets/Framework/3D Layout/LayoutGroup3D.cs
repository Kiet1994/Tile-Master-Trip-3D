using System.Collections.Generic;
using UnityEngine;

public enum LayoutStyle
{
    Linear,
    Grid,
    Euclidean,
    Radial
}

public enum LayoutAxis3D
{
    X,
    Y,
    Z
}

public enum LayoutAxis2D
{
    X,
    Y
}

public enum Alignment
{
    Min,
    Center,
    Max
}

[ExecuteAlways]
public class LayoutGroup3D : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> LayoutElements;

    [HideInInspector]
    public Vector3 ElementDimensions = Vector3.one;

    [HideInInspector]
    public float Spacing;

    [HideInInspector]
    public LayoutStyle Style;

    [HideInInspector]
    public int GridConstraintCount;

    [HideInInspector]
    public int SecondaryConstraintCount;

    [HideInInspector]
    [Tooltip("If max < 1, Max is infinity. Otherwise, ConstraintCount of Element cann't > max ")]
    public int MaxRemainingConstrainCount;

    [HideInInspector]
    public bool UseFullCircle;

    [HideInInspector]
    public float MaxArcAngle;

    [HideInInspector]
    public float Radius;

    [HideInInspector]
    public float StartAngleOffset;

    [HideInInspector]
    public bool AlignToRadius;

    [HideInInspector]
    public LayoutAxis3D LayoutAxis;

    [HideInInspector]
    public LayoutAxis3D SecondaryLayoutAxis;

    [HideInInspector]
    public LayoutAxis2D GridLayoutAxis;

    [HideInInspector]
    public List<Quaternion> ElementRotations;

    [HideInInspector]
    public Vector3 StartPositionOffset;

    [HideInInspector]
    public float SpiralFactor;

    [HideInInspector]
    public Alignment PrimaryAlignment;

    [HideInInspector]
    public Alignment SecondaryAlignment;

    [HideInInspector]
    public Alignment TertiaryAlignment;

    [HideInInspector]
    public bool NeedsRebuild = false;

    [Tooltip("Forces the layoutgroup to rebuild every frame while in Play mode.  WARNING: This may have performance implications for very large / complex layouts, use with caution")]
    public bool ForceContinuousRebuild = false;

    [Tooltip("If true, child gameobjects that are not activeSelf will be ignored for the layout.  Otherwise, inactive child gameobjects will be included in the layout.")]
    public bool IgnoreDeactivatedElements = true;

    private float timeMoveElement = 0.05f;
    private bool isBuildLastChildOnly = false;
    private float currentHeight = 0;
    private bool isAddElement;

    public void RebuildLayout()
    {
        PopulateElementsFromChildren();

        switch (Style)
        {
            case LayoutStyle.Linear:
                LinearLayout();
                break;

            case LayoutStyle.Grid:
                GridLayout();
                break;

            case LayoutStyle.Euclidean:
                EuclideanLayout();
                break;

            case LayoutStyle.Radial:
                RadialLayout();
                break;
        }

        NeedsRebuild = false;
    }

    public void LinearLayout()
    {
        Vector3 alignmentOffset = GetVectorForAxis(LayoutAxis);

        // Calculate alignment offset amount
        switch (PrimaryAlignment)
        {
            case Alignment.Min:
                alignmentOffset = Vector3.zero;
                break;

            case Alignment.Center:
                alignmentOffset *= -(LayoutElements.Count - 1) * (ElementDimensions.x + Spacing);
                break;

            case Alignment.Max:
                alignmentOffset *= -(LayoutElements.Count - 1) * (ElementDimensions.x + Spacing);
                break;
        }
        float height = 0;
        float heightCurElement = 0;
        float timeDelay = 0;
        for (int i = 0; i < LayoutElements.Count; i++)
        {
            var pos = Vector3.zero;
            Vector3 dimensions = ElementDimensions;
            switch (LayoutAxis)
            {
                case LayoutAxis3D.X:
                    pos.x = (float)i * (dimensions.x + Spacing);
                    break;

                case LayoutAxis3D.Y:
                    pos.y = height + Spacing;
                    height += heightCurElement;
                    break;

                case LayoutAxis3D.Z:
                    pos.z = (float)i * (dimensions.z + Spacing);
                    break;
            }
            LayoutElements[i].transform.localPosition = pos + StartPositionOffset + alignmentOffset;
            timeDelay += timeMoveElement;
            LayoutElements[i].transform.localRotation = Quaternion.identity;
        }
    }

    public void GridLayout()
    {
        for (int i = 0; i < LayoutElements.Count; i++)
        {
            LayoutElements[i].localRotation = Quaternion.identity;
            LayoutElements[i].localPosition = GetPositionByIndex(i);
        }
    }

    public void EuclideanLayout()
    {
        for (int i = 0; i < LayoutElements.Count; i++)
        {
            LayoutElements[i].localRotation = Quaternion.identity;
            LayoutElements[i].localPosition = GetPositionByIndex(i);
        }
    }

    public void RadialLayout()
    {
        Vector3 pos = Vector3.zero;
        float spiralSum = 0f;
        float spiralIncrement = SpiralFactor / LayoutElements.Count;

        if (UseFullCircle)
        {
            MaxArcAngle = 360f - 360f / LayoutElements.Count;
        }

        for (int i = 0; i < LayoutElements.Count; i++)
        {
            float angle = (float)i / (LayoutElements.Count - 1) * MaxArcAngle * Mathf.Deg2Rad;
            pos.x = Mathf.Cos(angle + Mathf.Deg2Rad * StartAngleOffset) * (Radius + spiralSum);
            pos.y = Mathf.Sin(angle + Mathf.Deg2Rad * StartAngleOffset) * (Radius + spiralSum);

            if (AlignToRadius)
            {
                Vector3 dir = transform.TransformDirection(pos);
                LayoutElements[i].up = dir;
            }

            LayoutElements[i].localPosition = pos + StartPositionOffset;
            spiralSum += spiralIncrement;
        }
    }

    public void PopulateElementsFromChildren()
    {
        if (LayoutElements == null)
        {
            LayoutElements = new List<Transform>();
        }

        if (ElementRotations == null)
        {
            ElementRotations = new List<Quaternion>();
        }

        LayoutElements.Clear();

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf || !IgnoreDeactivatedElements)
            {
                LayoutElements.Add(child);
            }
        }
    }

    public void RecordRotations()
    {
        if (LayoutElements == null)
        {
            return;
        }

        if (ElementRotations == null)
        {
            ElementRotations = new List<Quaternion>();
        }

        if (HasChildCountChanged())
        {
            PopulateElementsFromChildren();
        }

        ElementRotations.Clear();

        for (int i = 0; i < LayoutElements.Count; i++)
        {
            ElementRotations.Add(LayoutElements[i].localRotation);
        }
    }

    public void RestoreRotations()
    {
        if (LayoutElements == null || ElementRotations == null || LayoutElements.Count != ElementRotations.Count)
        {
            return;
        }

        for (int i = 0; i < LayoutElements.Count; i++)
        {
            LayoutElements[i].localRotation = ElementRotations[i];
        }
    }

    public bool HasChildCountChanged()
    {
        if (LayoutElements != null)
        {
            if (IgnoreDeactivatedElements)
            {
                if(GetNumActiveChildren() >= LayoutElements.Count)
                {
                    timeMoveElement = 0f;
                    isAddElement = true;
                }
                else
                {
                    timeMoveElement = 0.05f;
                    isAddElement = false;
                }

                return GetNumActiveChildren() != LayoutElements.Count;
            }
            else
            {
                if (transform.childCount >= LayoutElements.Count)
                {
                    timeMoveElement = 0f;
                    isAddElement = true;
                }
                else
                {
                    timeMoveElement = 0.05f;
                    isAddElement = false;
                }
                return transform.childCount != LayoutElements.Count;
            }
        }

        return false;
    }

    public int GetNumActiveChildren()
    {
        int num = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                num++;
            }
        }
        return num;
    }

    public Vector3 GetVectorForAxis(LayoutAxis3D Axis3D)
    {
        Vector3 axis = Vector3.zero;
        // Calculate alignment direction
        switch (Axis3D)
        {
            case LayoutAxis3D.X:
                axis = Vector3.right;
                break;

            case LayoutAxis3D.Y:
                axis = Vector3.up;
                break;

            case LayoutAxis3D.Z:
                axis = Vector3.forward;
                break;
        }

        return axis;
    }

    public Vector3 GetVectorForAxis(LayoutAxis2D Axis2D)
    {
        Vector3 axis = Vector3.zero;
        switch (Axis2D)
        {
            case LayoutAxis2D.X:
                axis = Vector3.right;
                break;

            case LayoutAxis2D.Y:
                axis = Vector3.up;
                break;
        }

        return axis;
    }

    public void Update()
    {
        if (isBuildLastChildOnly)
        {
            BuildLastChild();
        }
        else if (NeedsRebuild || HasChildCountChanged())
        {
            RebuildLayout();
        }
        else if (Application.IsPlaying(gameObject) && ForceContinuousRebuild)
        {
            RebuildLayout();
        }
    }

    public void OnTransformChildrenChanged()
    {
        if (IgnoreDeactivatedElements)
        {
            if (GetNumActiveChildren() > LayoutElements.Count)
            {
                if (!isBuildLastChildOnly)
                {
                    NeedsRebuild = true;
                }
                isAddElement = true;
            }
            else if(GetNumActiveChildren() < LayoutElements.Count)
            {
                if (!isBuildLastChildOnly)
                {
                    NeedsRebuild = true;
                }
                isAddElement = false;
            }
        }
        else
        {
            if (transform.childCount > LayoutElements.Count)
            {
                if (!isBuildLastChildOnly)
                {
                    NeedsRebuild = true;
                }
                isAddElement = true;
            }
            else if (transform.childCount < LayoutElements.Count)
            {
                if (!isBuildLastChildOnly)
                {
                    NeedsRebuild = true;
                }
                isAddElement = false;
            }
        }

    }

    public void PushLast(Transform transform)
    {
        isBuildLastChildOnly = true;
        transform.SetParent(this.transform);
        isAddElement = true;
    }

    public Transform PopLast()
    {
        if(transform.childCount == 0)
		{
            return null;
		}

        isBuildLastChildOnly = true;
        Transform lastChild = this.transform.GetChild(this.transform.childCount - 1);
        lastChild.SetParent(SimplePool.Instance.transform);
        isAddElement = false;
        return lastChild;
    }

    private void BuildLastChild()
    {
        PopulateLastChild();

        switch (Style)
        {
            case LayoutStyle.Linear:
                // LinearLayout();
                break;

            case LayoutStyle.Grid:
                GridLayout();
                break;

            case LayoutStyle.Euclidean:
                EuclideanLayoutLastOnly();
                break;

            case LayoutStyle.Radial:
                // RadialLayout();
                break;
        }

        isBuildLastChildOnly = false;
    }

    private void PopulateLastChild()
    {
        if (LayoutElements == null)
        {
            LayoutElements = new List<Transform>();
        }

        if (ElementRotations == null)
        {
            ElementRotations = new List<Quaternion>();
        }

        for (int i = Mathf.Max(transform.childCount - 1, 0); i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf || !IgnoreDeactivatedElements)
            {
                LayoutElements.Add(child);
            }
        }
    }

    private void EuclideanLayoutLastOnly()
    {
        if (LayoutElements.Count == 0)
            return;

        int index = LayoutElements.Count - 1;
        LayoutElements[index].localRotation = Quaternion.identity;
        LayoutElements[index].localPosition = GetPositionByIndex(index - (isAddElement?0:2));
    }

    private Vector3 GetPositionByIndex(int index)
    {
        if(index < 0)
        {
            index = 0;
        }
        Vector3 pos = Vector3.zero;

        int primaryCount = GridConstraintCount;
        int secondaryCount = 1;
        int tertiaryCount = 1;
        switch (Style)
        {
            case LayoutStyle.Linear:
                primaryCount = 1000;
                secondaryCount = Mathf.Max(1, Mathf.CeilToInt((float)LayoutElements.Count / primaryCount));
                break;

            case LayoutStyle.Grid:
                secondaryCount = Mathf.Max(1, Mathf.CeilToInt((float)LayoutElements.Count / primaryCount));
                break;

            case LayoutStyle.Euclidean:
                secondaryCount = SecondaryConstraintCount;
                tertiaryCount = Mathf.CeilToInt((float)LayoutElements.Count / (primaryCount * secondaryCount));
                break;

            case LayoutStyle.Radial:
                break;
        }

        // Bit mask to determine final driven axis (001 = X, 010 = Y, 100 = Z)
        int tertiaryAxisMask = 7;
        LayoutAxis3D tertiaryAxis = LayoutAxis3D.X;

        #region Determine Element Dimensions in Each Axis

        float pDim = 0f, sDim = 0f, tDim = 0f;

        switch (LayoutAxis)
        {
            case LayoutAxis3D.X:
                pDim = ElementDimensions.x;
                tertiaryAxisMask ^= 1;
                break;

            case LayoutAxis3D.Y:
                pDim = ElementDimensions.y;
                tertiaryAxisMask ^= 2;
                break;

            case LayoutAxis3D.Z:
                pDim = ElementDimensions.z;
                tertiaryAxisMask ^= 4;
                break;
        }

        switch (SecondaryLayoutAxis)
        {
            case LayoutAxis3D.X:
                sDim = ElementDimensions.x;
                tertiaryAxisMask ^= 1;
                break;

            case LayoutAxis3D.Y:
                sDim = ElementDimensions.y;
                tertiaryAxisMask ^= 2;
                break;

            case LayoutAxis3D.Z:
                sDim = ElementDimensions.z;
                tertiaryAxisMask ^= 4;
                break;
        }

        switch (tertiaryAxisMask)
        {
            case 1:
                tDim = ElementDimensions.x;
                break;

            case 2:
                tDim = ElementDimensions.y;
                break;

            case 4:
                tDim = ElementDimensions.z;
                break;
        }

        switch (tertiaryAxisMask)
        {
            case 1:
                tertiaryAxis = LayoutAxis3D.X;
                break;

            case 2:
                tertiaryAxis = LayoutAxis3D.Y;
                break;

            case 4:
                tertiaryAxis = LayoutAxis3D.Z;
                break;
        }

        #endregion Determine Element Dimensions in Each Axis

        Vector3 alignmentOffset = Vector3.zero;

        #region Calculate alignment offset vectors

        // Calculate primary alignment offset
        if (Style >= LayoutStyle.Linear)
        {
            switch (PrimaryAlignment)
            {
                case Alignment.Min:
                    break;

                case Alignment.Center:
                    alignmentOffset -= GetVectorForAxis(LayoutAxis) * (primaryCount - 1) * (pDim + Spacing) / 2f;
                    break;

                case Alignment.Max:
                    alignmentOffset -= GetVectorForAxis(LayoutAxis) * (primaryCount - 1) * (pDim + Spacing);
                    break;
            }
        }
        // Calculate secondary alignment offset
        if (Style >= LayoutStyle.Grid)
        {
            switch (SecondaryAlignment)
            {
                case Alignment.Min:
                    break;

                case Alignment.Center:
                    alignmentOffset -= GetVectorForAxis(SecondaryLayoutAxis) * (secondaryCount - 1) * (sDim + Spacing) / 2f;
                    break;

                case Alignment.Max:
                    alignmentOffset -= GetVectorForAxis(SecondaryLayoutAxis) * (secondaryCount - 1) * (sDim + Spacing);
                    break;
            }
        }
        // Calculate tertiary alignment offset
        if (Style >= LayoutStyle.Euclidean)
        {
            switch (TertiaryAlignment)
            {
                case Alignment.Min:
                    break;

                case Alignment.Center:
                    alignmentOffset -= GetVectorForAxis(tertiaryAxis) * (tertiaryCount - 1) * (tDim + Spacing) / 2f;
                    break;

                case Alignment.Max:
                    alignmentOffset -= GetVectorForAxis(tertiaryAxis) * (tertiaryCount - 1) * (tDim + Spacing);
                    break;
            }
        }

        #endregion Calculate alignment offset vectors

        int pDimMax = primaryCount;
        int sDimMax = secondaryCount;
        int psDimMax = pDimMax * sDimMax;
        int t = 1;

        if(psDimMax > 0)
        {
           t = index / psDimMax;
        }

        if (MaxRemainingConstrainCount > 0 && t > MaxRemainingConstrainCount - 1)
        {
            t = MaxRemainingConstrainCount - 1;
        }

        int s = (index % psDimMax) / pDimMax; 
        int p = (index % psDimMax) % pDimMax;

        float pOffset = (float)p * (pDim + Spacing);
        float sOffset = (float)s * (sDim + Spacing);
        float tOffset = (float)t * (tDim + Spacing);

        switch (LayoutAxis)
        {
            case LayoutAxis3D.X:
                pos.x = pOffset;
                break;

            case LayoutAxis3D.Y:
                if (p == 0)
                {
                    currentHeight = 0;
                }
                else
                {
                    currentHeight = pOffset;
                }
           
                pos.y = currentHeight;
                break;

            case LayoutAxis3D.Z:
                pos.z = pOffset;
                break;
        }

        switch (SecondaryLayoutAxis)
        {
            case LayoutAxis3D.X:
                pos.x = sOffset;
                break;

            case LayoutAxis3D.Y:               
                pos.y = sOffset;
                break;

            case LayoutAxis3D.Z:
                pos.z = sOffset;
                break;
        }

        switch (tertiaryAxisMask)
        {
            case 1:
                pos.x = tOffset;
                break;

            case 2:
                pos.y = tOffset;
                break;

            case 4:
                pos.z = tOffset;
                break;
        }
        return pos + StartPositionOffset + alignmentOffset;
    }

    public Vector3 GetNewPosition(int index = 0)
    {
        return transform.TransformPoint(GetPositionByIndex(LayoutElements.Count + index));
    }

    public List<Vector3> GetListNewPosition(int number)
    {
        List<Vector3> listHeadPos = new List<Vector3>();
        for(int i = 0; i < number; i++)
        {
            Vector3 newPos = transform.TransformPoint(GetPositionByIndex(LayoutElements.Count + i));
            listHeadPos.Add(newPos);
        }

        return listHeadPos;
    }
}