using Echo.Command;
using UnityEngine;

public class MoveCommand : ICommand
{
	public void Execute()
	{
		Debug.Log("MoveCommand Execute!");
	}
}
