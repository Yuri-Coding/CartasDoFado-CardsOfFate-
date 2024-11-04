using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	InitGame,           // Inicialização do jogo

	// Fases de Ações
	AwaitAction,		// Esperar todas as ações de todos os players.
	HandleActions,		// Processar efeitos de cartas.
	ShowResults,		// Exibir resultados das ações.

	// Fases de Votação
	VotingPhase,		// Fase de Votação para eliminar um jogador.
	ProcessVoteResults,	// Processar resultados da votação.

	// Fase Única
	EndPhase,			// Verifica condições finalização de jogo.

	// Finalização
	Win,				// Vitória
	Lose				// Derrota
}
