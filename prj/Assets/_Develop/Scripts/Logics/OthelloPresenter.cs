using System;
using ProgramDesignMock230211.Logics.Views;
using UniRx;
using UnityEngine;

namespace ProgramDesignMock230211.Logics
{
    /// <summary>
    ///     <see cref="OthelloModel" />と<see cref="IOthelloView" />を仲介するPresenter
    /// </summary>
    public sealed class OthelloPresenter : IDisposable
    {
        /// <summary>
        ///     MVPのModel部分
        /// </summary>
        private readonly OthelloModel _othelloModel;

        /// <summary>
        ///     MVPのView部分
        /// </summary>
        private readonly IOthelloView _othelloView;

        /// <summary>
        ///     MVP関連の購読一括解除用のCompositeDisposable
        /// </summary>
        private readonly CompositeDisposable _compositeDisposable = new();

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="gridSize">Gridの盤面サイズ</param>
        /// <param name="othelloView">View</param>
        public OthelloPresenter(Vector2Int gridSize, IOthelloView othelloView)
        {
            _othelloModel = new OthelloModel(gridSize);
            _othelloView = othelloView;
            _othelloModel.OnUpdatePlaceablePos.Subscribe(othelloView.DisplayPlaceablePos).AddTo(_compositeDisposable);
            _othelloModel.OnUpdatePiece.Subscribe(othelloView.UpdatePiece).AddTo(_compositeDisposable);
            _othelloView.OnGridSelected.Subscribe(x =>
                {
                    if (_othelloModel.TryPlacePiece(x) == false)
                    {
                        return;
                    }

                    if (_othelloModel.TryChangeTurn() == false)
                    {
                        Debug.Log("ゲーム終了");
                    }
                })
                .AddTo(_compositeDisposable);
            _othelloView.Initialize(gridSize);
            _othelloModel.InitializeBoard();
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}
