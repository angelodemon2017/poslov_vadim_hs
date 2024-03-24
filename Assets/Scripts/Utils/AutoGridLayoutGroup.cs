using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class AutoGridLayoutGroup : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private RectTransform _rectTransform;

    [Header("Кол-во ожидаемых элементов по определенному направлению")]
    [SerializeField] private int countRow = 2;
    [SerializeField] private int countColumn = 2;

    [Header("Является ли ячейка квадратной")]
    [SerializeField] private bool squareCell = false;

    [Header("Ширина пространства от крайнего элемента до края панели")]
    [SerializeField] private int widthPadding;
    [Header("Ширина пространства между элементами панели")]
    [SerializeField] private int widthSpacing;

    private void OnValidate()
    {
        if (_gridLayoutGroup == null)
        {
            _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        }
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }

    private void Awake()
    {
        UpdateGrid();
    }

    public void UpdateCells(int count)
    {
        var sqrtResult = Mathf.Sqrt(count);
        countRow = (int)sqrtResult;
        countColumn = countRow + ((sqrtResult - (int)sqrtResult == 0) ? 0 : 1);

        UpdateGrid();
    }

    public void UpdateGrid()
    {
        _gridLayoutGroup.padding = new RectOffset(widthPadding, widthPadding, widthPadding, widthPadding);
        _gridLayoutGroup.spacing = new Vector2(widthSpacing, widthSpacing);

        var width = _rectTransform.rect.width;
        var height = _rectTransform.rect.height;

        var widthCell = (width - widthPadding * 2 - widthSpacing * (countColumn - 1)) / countColumn;
        var heightCell = squareCell ? widthCell : (height - widthPadding * 2 - widthSpacing * (countRow - 1)) / countRow;
        if (heightCell * countRow + widthSpacing * (countRow - 1) + widthPadding * 2 > height)
        {
            heightCell = (height - widthPadding * 2 - widthSpacing * (countRow - 1)) / countRow;
            widthCell = squareCell ? heightCell : (width - widthPadding * 2 - widthSpacing * (countColumn - 1)) / countColumn;
        }

        _gridLayoutGroup.cellSize = new Vector2(widthCell, heightCell);
    }
}