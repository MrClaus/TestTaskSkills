using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI : MonoBehaviour
{
    [Header("Bar")]
    public GameObject bar;
    public Text price;
    public Text score;

    [Header("Buttons")]
    public Button[] BtnSchemes;
    public Button BtnGet;
    public Button BtnReturn;

    [Header("Schemes")]
    public GameObject[] Schemes;

    private int activeScheme = 0, activeSkill = 0;              // текущая активная схема и скилл (если выбран)
    private int activeSkillFullPath = 0;                        // длина пути активного скилла до базового
    private int activeSkillCountEnd = 0;                        // количество путей, ведущих к базе у активного скилла
    private List<SchemeState> states = new List<SchemeState>(); // лист состояний схем
    private List<int> arrayElementsLoopChain = new List<int>(); // лист элементов замкнутой цепи блоков
    private bool isGetSkill, isReturnSkill;                     // возможность использования опций для выбранного скилла

    private class SchemeState
    {
        public int score = 16;
        public int[] lockedState;
    }

    void Start()
    {
        LoadState();
    }

    // Загрузка состояний схем
    private void LoadState()
    {
        for (int i = 0; i < Schemes.Length; i++)
        {
            var state = new SchemeState();
            var array = Schemes[i].GetComponent<LineGenerate>().BlockList;
            state.lockedState = new int[array.Count];

            for (int j = 0; j < array.Count; j++)
                state.lockedState[j] = (IsBasisSkill(j, i)) ? 1 : 0;
            states.Add(state);
        }

        SelectScheme(activeScheme);
    }

    // Выбор схемы
    public void SelectScheme(int id)
    {
        for (int i = 0; i < Schemes.Length; i++)
        {
            Schemes[i].SetActive(false);
            BtnSchemes[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }

        BtnSchemes[id].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        score.text = "ОЧКИ: " + states[id].score;
        Schemes[id].SetActive(true);
        activeScheme = id;
    }

    // Открытие бара
    public void ClickBlock(int id)
    {
        bar.SetActive(true);
        activeSkill = id;
        isGetSkill = false;
        isReturnSkill = false;
        BtnGet.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        BtnReturn.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);        
        price.text = "СТОИМОСТЬ: " + id;

        int prevId = GetPrevId(id);
        int nextId = GetNextId(id);

        if ((IsOpenSkill(prevId) && !IsOpenSkill(id) && !IsBasisSkill(id)) ||
            (IsOpenSkill(nextId) && nextId != id && !IsOpenSkill(id) && !IsBasisSkill(id)))
        {
            BtnGet.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            isGetSkill = true;
        }
        else if ( IsOpenSkill(id) && 
            ((IsInChainLoop(id) && (id == GetElementLongPathInChainLoop(id))) || !IsInChain(id) || 
            (IsInChain(id) && activeSkillCountEnd >= GetBlockById(id).InId.Length)) )
        {
            BtnReturn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            isReturnSkill = true;
        }
    }

    // Закрытие бара
    public void ClickScreen()
    {
        bar.SetActive(false);
        activeSkill = 0;
    }

    // Получаем навык и тратим имеющиеся очочки
    public void ClickGetSkill()
    {
        if (isGetSkill && states[activeScheme].score >= activeSkill)
        {
            states[activeScheme].lockedState[activeSkill] = 1;
            states[activeScheme].score -= activeSkill;
            score.text = "ОЧКИ: " + states[activeScheme].score;
            GetBlockById(activeSkill).transform.Find("txt_block").GetComponent<Text>().text = "" + activeSkill;
            ClickBlock(activeSkill);
        }
    }

    // Забываем навык и возвращаем потраченные на него очочки
    public void ClickReturnSkill()
    {
        if (isReturnSkill)
        {
            states[activeScheme].lockedState[activeSkill] = 0;
            states[activeScheme].score += activeSkill;
            score.text = "ОЧКИ: " + states[activeScheme].score;
            GetBlockById(activeSkill).transform.Find("txt_block").GetComponent<Text>().text = "???";
            ClickBlock(activeSkill);
        }
    }

    // Забываем все приобретенные навыки, кроме базовых
    public void ClickReturnAllSkill()
    {
        int summ = 0;
        for (int i = 0; i < states[activeScheme].lockedState.Length; i++)
        {
            if (states[activeScheme].lockedState[i] == 1 && !IsBasisSkill(i))
            {
                GetBlockById(i).transform.Find("txt_block").GetComponent<Text>().text = "???";
                states[activeScheme].lockedState[i] = 0;
                summ += i;
            }
        }

        states[activeScheme].score += summ;
        score.text = "ОЧКИ: " + states[activeScheme].score;
        ClickScreen();
    }

    // Зарабатываем очочки
    public void ClickAddScore()
    {
        states[activeScheme].score++;
        score.text = "ОЧКИ: " + states[activeScheme].score;
    }

    // [Вспомогательный метод] Если текущий скилл открыт, то возвращает true, иначе - false
    private bool IsOpenSkill(int idBlock)
    {
        return (states[activeScheme].lockedState[idBlock] == 1) ? true : false;
    }

    // [Вспомогательный метод] Если текущий скилл базовый, то возвращает true, иначе - false
    private bool IsBasisSkill(int idBlock)
    {
        return IsBasisSkill(idBlock, activeScheme);
    }

    private bool IsBasisSkill(int idBlock, int current)
    {
        // изначально не так понял описание, и предполагал, что базовыми также могут считаться непосредственно связанные с Базой скиллы. Поправил
        /*foreach (int mId in GetBlockById(idBlock).InId)
            if (mId == 0)
                return true;*/

        return (idBlock == 0) ? true : false;
    }

    // [Вспомогательный метод] Возвращает индекс предыдущего скилла по отношению к выбранному текущему
    private int GetPrevId(int id)
    {
        List<int> arr = new List<int>();
        foreach (int mId in GetBlockById(id).InId)
            if (mId < id)
                arr.Add(mId);
        for (int i = 0; i < arr.Count; i++)
            if (states[activeScheme].lockedState[arr[i]] == 1)
                return arr[i];

        return (arr.Count > 0) ? arr[0] : id;
    }

    // [Вспомогательный метод] Возвращает индекс следующего скилла по отношению к выбранному текущему
    private int GetNextId(int id)
    {
        List<int> arr = new List<int>();
        foreach (int mId in GetBlockById(id).InId)
            if (mId > id)
                arr.Add(mId);
        for (int i = 0; i < arr.Count; i++)
            if (states[activeScheme].lockedState[arr[i]] == 1)
                return arr[i];

        return (arr.Count > 0) ? arr[0] : id;
    }

    // [Вспомогательный метод] Если текущий блок находится в цепи между изученными навыками, возвращает true, иначе - false
    private bool IsInChain(int id)
    {
        int cntOpenSkill = 0;
        foreach (int mId in GetBlockById(id).InId)
            if (IsOpenSkill(mId))
                cntOpenSkill++;

        return (cntOpenSkill > 1);
    }

    // [Вспомогательный метод] Если текущий блок находится в зацикленной цепи изученных навыков, возвращает true, иначе - false
    private bool IsInChainLoop(int id)
    {
        bool isEndChain = false, isLoop = false, isFirstRead = false;

        List<int> mField = new List<int>() { id };
        List<List<List<int>>> nList = new List<List<List<int>>>();        
        nList.Add(new List<List<int>>() { mField });

        while (!isEndChain)
        {
            List<int> field = new List<int>();
            List<List<int>> mList = new List<List<int>>();

            for (int k = 0; k < mField.Count; k++)
            {
                int elem = mField[k];
                List<int> toField = new List<int>();

                if (elem < 0)
                {
                    toField.Add(-1);
                    field.Add(-1);
                }
                else if (elem == id && isFirstRead || elem == 0)
                {                    
                    toField.Add(elem);
                    field.Add(elem);
                }
                else
                {
                    foreach (int mId in GetBlockById(elem).InId)
                    {
                        int val = (states[activeScheme].lockedState[mId] == 1 && isNoRepeat(k, mId)) ? mId : ((mId == 0) ? 0 : -1);
                        toField.Add(val);
                        field.Add(val);
                    }
                }

                isFirstRead = true;
                mList.Add(toField);
            }
            
            nList.Add(mList);
            mField = field;
            isEndChain = isEndCycleCheck();
        }

        bool isEndCycleCheck()
        {
            int sum = 0, sumLoop = 0, sumNo = 0, sumEnd = 0;
            List<List<int>> arr = nList[nList.Count - 1];

            for (int i = 0; i < arr.Count; i++)
            {
                for (int j = 0; j < arr[i].Count; j++)
                {
                    if (arr[i][j] == id)
                    {
                        isLoop = true;
                        sumLoop++;
                    }
                    if (arr[i][j] == 0)
                        sumEnd++;
                    if (arr[i][j] == -1)
                        sumNo++;

                    sum++;
                }
            }

            return (sum == sumNo + sumLoop + sumEnd);
        }
        
        bool isNoRepeat(int crntId, int chkDig)
        {
            if (nList.Count > 1)
            {
                List<List<int>> arr = nList[nList.Count - 1];
                int fId = 0;
                for (int i = 0; i < arr.Count; i++)
                {
                    fId += arr[i].Count;
                    if (crntId <= fId - 1)
                    {
                        fId = i;
                        break;
                    }
                }

                int sum = 0;
                int val = -1;
                arr = nList[nList.Count - 2];
                for (int i = 0; i < arr.Count; i++)
                {
                    for (int j = 0; j < arr[i].Count; j++)
                    {
                        if (sum == fId)
                        {
                            val = arr[i][j];
                            break;
                        }
                        sum++;
                    }
                    if (val != -1)
                        break;
                }

                return (chkDig != val);
            }

            return true;
        }

        // подсчет длины пути до базы
        bool isNotWriteLenFullPath = true;
        for (int g = 0; g < nList.Count; g++)
        {
            List<List<int>> mArr = nList[g];

            for (int i = 0; i < mArr.Count; i++)
            {
                for (int j = 0; j < mArr[i].Count; j++)
                {
                    if (mArr[i][j] == 0 && isNotWriteLenFullPath)
                    {
                        isNotWriteLenFullPath = false;
                        activeSkillFullPath = g;
                    }
                }
            }
        }
        
        // подсчет количества путей, ведущих к базе
        activeSkillCountEnd = 0;
        List<List<int>> arr = nList[nList.Count - 1];
        List<int> arrIds = new List<int>();
        List<int> arrElem = new List<int>();
        for (int i = 0; i < arr.Count; i++)
            for (int j = 0; j < arr[i].Count; j++)
            {
                if (arr[i][j] == id)
                {
                    arrElem.Add(id);
                    arrIds.Add(i);
                }
                if (arr[i][j] == 0)
                    activeSkillCountEnd++;
            }
                                 
        // вспомогательный список всех активных скиллов в текущей зацикленной цепи
        foreach (int crntId in arrIds)
        {
            int mCrntId = crntId;
            for (int t = nList.Count - 2; t > 0; t--)
            {
                int sum = 0, check = 0;
                arr = nList[t];
                for (int mT = 0; mT < arr.Count; mT++)
                {
                    for (int mD = 0; mD < arr[mT].Count; mD++)
                    {
                        if (sum == mCrntId)
                        {
                            arrElem.Add(arr[mT][mD]);
                            mCrntId = mT;
                            check = 1;
                            break;
                        }                            
                        sum++;
                    }
                    if (check == 1)
                        break;
                }
            }
        }
        
        arrayElementsLoopChain = arrElem;
        if (isNotWriteLenFullPath)
            activeSkillFullPath = 0;

        return isLoop;
    }

    // [Вспомогательный метод] Возвращает индекс блока в зацикленной цепи с самым длинным путём до базы
    private int GetElementLongPathInChainLoop(int id)
    {
        if (IsInChainLoop(id))
        {
            int[] list = new int[arrayElementsLoopChain.Count];
            int[] len = new int[arrayElementsLoopChain.Count];
            arrayElementsLoopChain.CopyTo(list);

            for (int i = 0; i < list.Length; i++)
            {
                int el = list[i];
                IsInChainLoop(el);
                len[i] = activeSkillFullPath;
            }

            Array.Sort(len, list);
            IsInChainLoop(id);
            
            return list[len.Length - 1];
        }

        return 0;
    }

    // [Вспомогательный метод] Возвращает экземпляр класса Блок (объекта Блок-схемы) по его id
    private Block GetBlockById(int id)
    {
        var array = Schemes[activeScheme].GetComponent<LineGenerate>().BlockList;
        foreach (GameObject block in array)
        {
            Block obj = block.GetComponent<Block>();
            if (obj.Id == id)
                return obj;
        }

        return null;
    }    
}
