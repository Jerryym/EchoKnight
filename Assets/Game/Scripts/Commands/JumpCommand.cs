using Echo;
using Echo.Command;
using UnityEngine;

public class JumpCommand : ICommand
{
    private PlayerController m_player;

    public JumpCommand(PlayerController player)
    {
        m_player = player;
    }

    public void Execute()
    {
		m_player.Jump();
    }
}
