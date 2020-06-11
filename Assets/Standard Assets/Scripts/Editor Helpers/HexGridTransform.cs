#if UNITY_EDITOR
using UnityEngine;
using GridGame;

public class HexGridTransform : EditorScript
{
	public Transform trs;

	public override void DoEditorUpdate ()
	{
		trs.localPosition = GameManager.GetSingleton<GameManager>().grid.GetCellCenterWorld(GameManager.GetSingleton<GameManager>().grid.WorldToCell(trs.localPosition));
	}
}
#endif