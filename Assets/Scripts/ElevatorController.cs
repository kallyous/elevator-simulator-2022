using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] public Transform Andar1;
    [SerializeField] public Transform Andar2;
    [SerializeField] public Transform Andar3;
    [SerializeField] public Text DebugTexto;
    [SerializeField] public InputField NovoPrograma;
    [SerializeField] public float Velocidade = 2.0f;
    [SerializeField] public float Espera = 1.0f;

    private string game_name = "Elevator Simulator 2022";
    private string game_alias = "Edt Covid-19, O retorno";

    private string program;
    private int current_floor;
    private int next_floor;
    private bool moving;
    private float elapsed_wait;
    private Transform target_floor;
    private List<Transform> available_floors;

    // FSM do elevador. Consome proxima letra e executa ação correspondente.
    private void FsmParse()
    {
        // Retorna se o programa estiver vazio.
        if (program.Length == 0) return;

        // Lê proxima letra do programa.
        var action = program[0];
        var next = -1;
        
        if (action == 's')
        {
            next = current_floor + 1;
        }
        
        else if (action == 'd')
        {
            next = current_floor - 1;
        }

        else
        {
            DebugTexto.text = $"ERRO!\nLetra inválida: {action}";
            return;
        }
        
        if (next > available_floors.Count) next = available_floors.Count;    
        if (next < 1) next = 1;
        
        setTargetFloor(next);
        program = program.Substring(1, program.Length - 1);
    }

    // Carrega programa escrito na entrada.
    public void loadProgram()
    {
        if (!moving) setProgram(NovoPrograma.text);
    }
    
    // Gera pequenos "programas" dos botões do elevador, reconhecíveis pela FSM.
    public void goToFloor(int next)
    {
        // O estado da máquina é indeterminado durante o movimento. 
        if (moving) return;
        
        // Evita o erro de fazer um botão que envie para andar inexistente.
        if (next < 1 || next > available_floors.Count)
        {
            DebugTexto.text = $"Andar {next} não existe.";
            return;
        }

        if (current_floor == 1)
        {
            if (next == 2) setProgram("s");
            else if (next == 3) setProgram("ss");
        }
        else if (current_floor == 2)
        {
            if (next == 1) setProgram("d");
            else if (next == 3) setProgram("s");
        }
        else // (current_floor == 3)
        {
            if (next == 1) setProgram("dd");
            else if (next == 2) setProgram("d");
        }
    }

    // Define novo programa do elevador. Ignora se elevador tiver movendo-se.
    private void setProgram(string prog)
    {
        if (!moving)
        {
            program = prog;
            DebugTexto.text = program;
        }
    }

    // Define alvo de locomoção do elevador.
    public void setTargetFloor(int floor_num)
    {
        next_floor = floor_num;
        target_floor = available_floors[floor_num - 1];
    }
    
    // Configuração inicial.
    private void Start()
    {
        available_floors = new List<Transform>();
        available_floors.Add(Andar1);
        available_floors.Add(Andar2);
        available_floors.Add(Andar3);
        target_floor = Andar1;
        current_floor = 1;
        next_floor = 1;
        program = "";
        moving = false;
        elapsed_wait = 0.0f;
        DebugTexto.text = game_name;
    }
    
    // Laço do jogo.
    private void Update()
    {
        // Se destino já foi alcançado, usando aproximação de 0.005 unidades, lê proxima letra.
        if (Vector3.Distance(transform.position, target_floor.position) < 0.005f)
        {
            moving = false;
            elapsed_wait = 0.0f;
            current_floor = next_floor;
            FsmParse();
            return;
        }

        
        if (elapsed_wait > Espera)
        {
            // Aproximação:
            moving = true;
            var step =  Velocidade * Time.deltaTime; // Cáculo do quanto mover.
            transform.position = Vector3.MoveTowards(transform.position, target_floor.position, step);
        }

        else
        {
            elapsed_wait += Time.deltaTime;
        }
        
    }
}
