using ProgramDesignMock230211.Grids;
using ProgramDesignMock230211.Grids.Views;
using UnityEngine;

namespace ProgramDesignMock230211
{
    /// <summary>
    ///     GameManager
    /// </summary>
    public sealed class GameManagerMono : MonoBehaviour
    {
        /// <summary>
        ///     Gridの3dView
        /// </summary>
        [SerializeField] private Grid3dViewMono _grid3dView;

        /// <summary>
        ///     Gridのサイズ
        /// </summary>
        [SerializeField] private Vector2Int _gridSize;

        /// <summary>
        ///     Awake関数
        /// </summary>
        private void Awake()
        {
            var presenter = new GridPresenter(_gridSize, _grid3dView);
        }
    }
}
