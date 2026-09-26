using Echo;
using UnityEngine;

public class MoveCommand : ICommand
{
	private PlayerController m_player;
	private Vector2 m_input;

	public MoveCommand(PlayerController player, Vector2 input)
	{
		m_player = player;
		m_input = input;
	}

	public void Execute()
	{
		m_player.Move(m_input);
	}
}
