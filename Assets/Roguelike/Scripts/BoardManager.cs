using System.Collections;
using UnityEngine;
using System;
//Permite-nos usar Listas.
using System.Collections.Generic;
//Diz ao Random para usar o gerador de números aleatórios do Unity Engine.
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // O uso de Serializable nos permite incorporar uma classe com subpropriedades no inspetor.
    [Serializable]
    public class Count
    {
        public int minimum;                 //Valor mínimo para a classe Count.
        public int maximum;                 //Valor máximo para a classe Count.

        //Construtor de atribuição.
        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    public int columns = 8;                 //Número de colunas em nosso tabuleiro de jogo.
    public int rows = 8;                    //Número de linhas em nosso tabuleiro de jogo.

    
    //Limite inferior e superior para nosso número aleatório de paredes por nível.
    public Count wallCount = new Count (5,9);
    //Limite inferior e superior para nosso número aleatório de alimentos por nível.
    public Count foodCount = new Count (1,5);
    public GameObject exit;                 //Prefab para gerar saida.
    public GameObject[] floorTiles;         //Array de pisos pré-fabricados.
    public GameObject[] wallTiles;          //Array de pré-fabricados de parede.
    public GameObject[] foodTiles;          //Array de alimentos pré-fabricados.
    public GameObject[] enemyTiles;         //Array de prefabs inimigos.
    public GameObject[] outerWallTiles;     //Array de blocos pré-fabricados externos.

    //Uma variável para armazenar uma referência à transformação do nosso objeto Board.
    private Transform boardHolder;
    //Uma lista de possíveis locais para colocar as peças.
    private List <Vector3> gridPositions = new List<Vector3>();

    //Limpa nossa lista gridPositions e a prepara para gerar um novo quadro.
    void InitialiseList() 
    {   
        //Limpa nossa lista gridPositions.
        gridPositions.Clear();
        //Loop através do eixo x (columns).
        for (int x = 1; x < columns -1; x++)
        {
            //Dentro de cada coluna, percorre o eixo y (rows).
            for (int y = 1; y < rows -1; y++)
            {
                //A cada índice adicione um novo Vector3 à nossa lista com as coordenadas x e y dessa posição.
                gridPositions.Add(new Vector3(x,y,0f));
            }
        }
    }


    //Configura as paredes externas e o piso (Background) do tabuleiro do jogo.
    void BoardSetup()
    {
        //Instanciar Board e definir boardHolder para seu transform.
        boardHolder = new GameObject ("Board").transform;

        //Loop ao longo do eixo x, começando em -1 (para preencher o canto) com pisos ou ladrilhos de borda externa.
        for (int x = -1; x < columns + 1; x++)
        {
            //Loop ao longo do eixo y, começando em -1 para colocar ladrilhos de piso ou parede externa.
            for (int y = -1; y < rows + 1; y++)
            {
                //Escolha um ladrilho aleatório de nossa matriz de ladrilhos pré-fabricados e prepare-se para instanciá-lo.
                GameObject toInstantiate = floorTiles[Random.Range(0 , floorTiles.Length)];

                //Verifique se a posição atual está na borda do tabuleiro, em caso afirmativo, escolha uma parede externa pré-fabricada aleatória de nossa matriz de ladrilhos de parede externa.
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance = Instantiate(toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

                //Defina o pai de nossa instância de objeto recém-instanciada para boardHolder, isso é apenas organizacional para evitar hierarquia desordenada.
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition ()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range (0, gridPositions.Count);

        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt (randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }

    
    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range (minimum, maximum+1);

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for(int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

       //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene (int level)
    {
        //Creates the outer walls and floor.
        BoardSetup ();

        //Reset our list of gridpositions.
        InitialiseList ();

        //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);

        //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

        //Determine number of enemies based on current level number, based on a logarithmic progression
        int enemyCount = (int)Mathf.Log(level, 2f);

        //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);

        //Instantiate the exit tile in the upper right hand corner of our game board
        Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
