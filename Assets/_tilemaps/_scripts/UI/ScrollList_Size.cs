using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollList_Size : MonoBehaviour
{

    [HideInInspector]
    public int numChildern = 0;
    private VerticalLayoutGroup verticalLayout;


    public void Start()
    {
        verticalLayout = GetComponent<VerticalLayoutGroup>();
    }

    public void SetHeight()
    {
        //How many kids do we have?
        var _myRect = this.GetComponent<RectTransform>();
        var _myHeight = this.GetComponent<RectTransform>().rect;
        var _children = this.transform.childCount;
        float _height = 0;

        //Util.WriteDebugLog( string.Format( "Start Width {0} Height {1}", _myRect.rect.width, _myRect.rect.height ) );
        for (int i = 0; i < _children; i++)
        {
            var _child = this.transform.GetChild(i);

            _height += _child.GetComponent<RectTransform>().rect.height;
            //Util.WriteDebugLog( string.Format( "Children {0} Height {1}", _child.name, _height ) );
        }

        //_myRect = _height;
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(_myRect.rect.width, _height);
        //Util.WriteDebugLog( string.Format( "Height is {0} for Payload list, with {1} children", _myRect, _children ) );
    }

    public void SetHeightNew(float newHeight)
    {
        var RT = GetComponent<RectTransform>();
        var rect = RT.rect;

        //IF we don't clone the sizeDelta then the width will be all fucked up
        var v = RT.sizeDelta;
        v.y = newHeight;

        RT.sizeDelta = v;
    }

    /// <summary>
    /// Make sure the UI is properly sized
    /// </summary>
    void LateUpdate()
    {
        numChildern = this.transform.childCount;
        var RT = GetComponent<RectTransform>();

        if (numChildern == 0)
        {
            SetHeightNew(0);
            return;
        }

        var childHeight = this.transform.GetChild(0).GetComponent<RectTransform>().rect.height;

        //Where to add the vertical spacing to the child height
        childHeight += verticalLayout.spacing;

        var totalChildHeight = childHeight * numChildern;

        //We need to account for padding

        totalChildHeight += verticalLayout.padding.top + verticalLayout.padding.bottom;

        SetHeightNew(totalChildHeight);

        //Util.WriteDebugLog(string.Format("Start Width {0} Height {1} Total child Height {2}", RT.rect.width ,RT.rect.height, totalChildHeight));

    }



}
