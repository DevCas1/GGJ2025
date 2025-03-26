using UnityEngine;

public class AspectRatioManager : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int _targetAspectRatioX = 16;
    [SerializeField, Min(1)]
    private int _targetAspectRatioY = 9;

    [SerializeField]
    private Camera _camera;

    private void Start()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = _targetAspectRatioX / _targetAspectRatioY;

        // determine the game window's current aspect ratio
        float windowaspect = Screen.width / Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;


        Rect rect = _camera.rect;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1)
        {
            rect.width = 1;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1 - scaleheight) / 2;
        }
        else // add pillarbox
        {
            float scalewidth = 1 / scaleheight;

            rect.width = scalewidth;
            rect.height = 1;
            rect.x = (1 - scalewidth) / 2;
            rect.y = 0;
        }

        _camera.rect = rect;

        // if (scaleheight < 1)
        // {
        //     Rect rect = _camera.rect;

        //     rect.width = 1;
        //     rect.height = scaleheight;
        //     rect.x = 0;
        //     rect.y = (1 - scaleheight) / 2;

        //     _camera.rect = rect;
        // }
        // else // add pillarbox
        // {
        //     float scalewidth = 1 / scaleheight;

        //     Rect rect = _camera.rect;

        //     rect.width = scalewidth;
        //     rect.height = 1;
        //     rect.x = (1 - scalewidth) / 2;
        //     rect.y = 0;

        //     _camera.rect = rect;
        // }
    }
}
