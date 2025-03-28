using System.Collections;
using UnityEngine;

public class Robot6 : MonoBehaviour
{
  
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float raioDaRoda = 1.0f; // Controla a distância percorrida
    public float raioDoCirculo = 0.5f; // Tamanho visual do robô
    public float velocidadeBase = 0.1f; // Começa mais lento
    public float fatorVelocidade = 10.0f; // Fator para aumentar a velocidade com o raio da roda,alcance que cada rotação da roda cobre no chão.
    private float tamanhoMinimo = 0.1f;
    private Vector2Int position;
    private int direction; // 0 = norte, 1 = leste, 2 = sul, 3 = oeste
    private CircleCollider2D circleCollider;
    private Rigidbody2D rb;
     public BatteryManager batteryManager;
    public float batteryConsumption = 5f; // Quantidade de bateria gasta por movimento


    void Start()
    {
        position = new Vector2Int(gridWidth / 2, gridHeight / 2); // Posição inicial (centro)
        direction = 0; // Norte
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        AjustarTamanho();
        UpdatePosition();
        UpdateRotation();
        // Certifique-se de que o BatteryManager está configurado
        if (batteryManager == null)
        {
            batteryManager = GetComponent<BatteryManager>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(1); // Movimenta para frente
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(-1); // Movimenta para trás
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TurnLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TurnRight();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Pressione espaço para aumentar o raio
        {
            if (CanIncreaseSize()) // Verifica se pode aumentar o tamanho
            {
                raioDoCirculo += 0.1f;
                AjustarTamanho();
            }
        }
        else if (Input.GetKeyDown(KeyCode.B)) // Diminuir o tamanho ao pressionar a tecla "B"
        {
            if (raioDoCirculo > tamanhoMinimo)
            {
                raioDoCirculo -= 0.1f; // Diminui o raio do círculo
                AjustarTamanho(); // Ajusta o tamanho visual do robô
            }
        }
    }

    void AjustarTamanho()
    {
        transform.localScale = new Vector3(raioDoCirculo * 2, raioDoCirculo * 2, 1);
        // Ajuste do collider
        if (circleCollider != null)
        {
            circleCollider.radius = raioDoCirculo / transform.localScale.x;
            circleCollider.offset = Vector2.zero;
        }
    }

    void Move(int directionMultiplier)
{
    // Primeiro, verificamos se ainda há bateria suficiente
    if (batteryManager.currentBattery <= 0)
    {
        Debug.Log("Bateria insuficiente para mover.");
        return; // Impede o movimento se a bateria estiver esgotada
    }

    Vector2Int nextPosition = position;

    // Calcular a velocidade com base no tamanho da roda
    float velocidadeAjustada = velocidadeBase * fatorVelocidade * raioDaRoda;

    // Atualiza a próxima posição baseada na direção
    switch (direction)
    {
        case 0: if (position.y < gridHeight - 1) nextPosition.y += directionMultiplier; break; // Norte
        case 1: if (position.x < gridWidth - 1) nextPosition.x += directionMultiplier; break; // Leste
        case 2: if (position.y > 0) nextPosition.y -= directionMultiplier; break; // Sul
        case 3: if (position.x > 0) nextPosition.x -= directionMultiplier; break; // Oeste
    }

    // Verifica se a célula não está ocupada e se o robô tem energia suficiente para se mover
    if (!FindObjectOfType<GridManager>().IsCellOccupied(nextPosition))
    {
        // Reduz a bateria antes de iniciar o movimento
        batteryManager.DecreaseBattery((int)batteryConsumption);

        // Inicia o movimento suave
        StartCoroutine(SmoothMove(nextPosition, velocidadeAjustada));
    }
}


    // Interpolação
    IEnumerator SmoothMove(Vector2Int targetPosition, float speed)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, 0);
        float t = 0; // Tempo de interpolação

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            rb.MovePosition(Vector3.Lerp(startPosition, endPosition, t)); // Usando Rigidbody para mover
            yield return null; // Espera o próximo frame
        }

        position = targetPosition;
    }

    void TurnLeft()
    {
        direction = (direction + 3) % 4; // Gira à esquerda
        UpdateRotation();
    }

    void TurnRight()
    {
        direction = (direction + 1) % 4; // Gira à direita
        UpdateRotation();
    }

    void UpdatePosition()
    {
        transform.position = new Vector3(position.x, position.y, 0);
    }

    void UpdateRotation()
    {
        switch (direction)
        {
            case 0: transform.rotation = Quaternion.Euler(0, 0, 0); break; // Norte
            case 1: transform.rotation = Quaternion.Euler(0, 0, -90); break; // Leste
            case 2: transform.rotation = Quaternion.Euler(0, 0, 180); break; // Sul
            case 3: transform.rotation = Quaternion.Euler(0, 0, 90); break; // Oeste
        }
    }

    bool CanIncreaseSize()
    {
        // Verifica se há obstáculos próximos
        Vector2Int checkPosition = position;

        return !FindObjectOfType<GridManager>().IsCellOccupied(new Vector2Int(checkPosition.x, checkPosition.y + 1)) && // Norte
               !FindObjectOfType<GridManager>().IsCellOccupied(new Vector2Int(checkPosition.x, checkPosition.y - 1)) && // Sul
               !FindObjectOfType<GridManager>().IsCellOccupied(new Vector2Int(checkPosition.x + 1, checkPosition.y)) && // Leste
               !FindObjectOfType<GridManager>().IsCellOccupied(new Vector2Int(checkPosition.x - 1, checkPosition.y)); // Oeste
    }
    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("RechargePoint"))
    {
        batteryManager.currentBattery = batteryManager.maxBattery;
        batteryManager.UpdateBatteryUI(); // Atualiza a barra visual no Canvas
    }
}
}

