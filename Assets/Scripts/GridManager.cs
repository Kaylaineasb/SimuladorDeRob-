using UnityEngine;
public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public GameObject obstaclePrefab; // Prefab para os obstáculos
    public Transform obstaclesParent; // Objeto pai para organizar os obstáculos na cena
    public GameObject linePrefab; // Prefab para as linhas do grid
    public Material lineMaterial; 
    private Vector2Int rechargePointPosition; // Posição do ponto de recarga


    private bool[,] grid; // Matriz para representar o grid

    void Start()
    {
        Camera.main.orthographicSize = 7; // Aumenta o campo de visão da câmera
        // Inicializa o grid
        grid = new bool[gridWidth, gridHeight];
        GameObject rechargePoint = GameObject.FindGameObjectWithTag("RechargePoint");
        if (rechargePoint != null)
        {
            rechargePointPosition = new Vector2Int(Mathf.FloorToInt(rechargePoint.transform.position.x), Mathf.FloorToInt(rechargePoint.transform.position.y));
            grid[rechargePointPosition.x, rechargePointPosition.y] = false; // Marca como livre
        }
        // Desenha o grid
        DrawGrid();

        // Coloca alguns obstáculos aleatórios no grid
        for (int i = 0; i < 10; i++)
        {
            int x = Random.Range(0, gridWidth);
            int y = Random.Range(0, gridHeight);

            // Verifica se a célula já está ocupada por um obstáculo ou se está no centro do grid
            if (grid[x, y] || (x == gridWidth / 2 && y == gridHeight / 2) || (x == rechargePointPosition.x && y == rechargePointPosition.y)) continue; // Pula se a célula já está ocupada ou é o centro
            grid[x, y] = true;

            // Instancia um obstáculo na cena
            GameObject obstacle = Instantiate(obstaclePrefab, new Vector3(x, y, 0), Quaternion.identity);
            obstacle.transform.parent = obstaclesParent; //relação pai/filho

            // Ajusta o collider do obstáculo
            BoxCollider boxCollider = obstacle.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.size = new Vector3(1, 1, 1);
                boxCollider.center = new Vector3(0, 0, 0);
            }
        }
    }

    // Função para desenhar o grid com linhas
    void DrawGrid()
    {
        // Desenha as linhas verticais
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x, 0, 0);
            Vector3 end = new Vector3(x, gridHeight, 0);
            DrawLine(start, end);
        }

        // Desenha as linhas horizontais
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(0, y, 0);
            Vector3 end = new Vector3(gridWidth, y, 0);
            DrawLine(start, end);
        }
    }

    // Função para instanciar uma linha
    void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject line = Instantiate(linePrefab);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; //números de pontos
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.1f; //largura inicial
        lineRenderer.endWidth = 0.1f;

        lineRenderer.material = lineMaterial;
    }

    public bool IsCellOccupied(Vector2Int position)
    {
        return grid[position.x, position.y];
    }
}
