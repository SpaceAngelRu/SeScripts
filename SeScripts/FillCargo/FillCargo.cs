#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

#endregion

namespace SeScripts.FillCargo
{
public sealed class Program : MyGridProgram
{
    #region Const

    private const int POWER_CELL_AMOUNT = 100; //Аккумулятор
    private const int BULLETPROOF_GLASS_AMOUNT = 100; //Бронированное стекло
    private const int LARGE_STEEL_TUBE_AMOUNT = 1000; //Большая стальная труба
    private const int GIRDER_AMOUNT = 100; //Балка
    private const int THRUST_COMPONENTS_AMOUNT = 0; //Детали ускорителя
    private const int COMPUTER_AMOUNT = 300; //Компьютер
    private const int SMALL_STEEL_TUBE_AMOUNT = 1000; //Маленькая стальная трубка
    private const int MEDICAL_COMPONENTS_AMOUNT = 100; //Медицинские компоненты
    private const int DETECTOR_COMPONENTS_AMOUNT = 100; //Компоненты детектора
    private const int GIR_GRAVITY_GENERATOR_COMPONENTS_DER_AMOUNT = 100; //Компоненты гравитационного генератора
    private const int RADIO_COMMUNICATION_COMPONENTS_AMOUNT = 100; //Комплектующие для радио-связи
    private const int REACTOR_COMPONENTS_AMOUNT = 1000; //Компоненты реактора
    private const int MOTOR_AMOUNT = 1000; //Мотор
    private const int INTERIOR_PLATE_AMOUNT = 1000; //Пластина
    private const int METAL_GRID_AMOUNT = 3000; //Решетка
    private const int STEEL_PLATE_AMOUNT = 3000; //Стальная пластина
    private const int CONSTRUCTION_COMPONENT_AMOUNT = 1000; //Строительный компонент
    private const int SOLAR_CELL_AMOUNT = 100; //Солнечная батарея
    private const int DISPLAY_AMOUNT = 100; //Экран

    //==================================================================================

    #region Amount settings

    //Сколько положить в целеовой контейнер
    private const int MAX_AMOUT = 100;

    #endregion

    #endregion

    #region Static

    private static readonly StringBuilder MsgsBuffer = new StringBuilder();

    #endregion

    #region Private fields

    private readonly string[] _signs = { "\\", "|", "/", "--" };
    private int _curSign = 0;

    private bool _isConnect = false;

    #endregion

    #region Construct

    public Program()
    { }

    #endregion

    #region Public

    public void Main(string args)
    {
        var cargoContainers = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers);

        var cargoDestinations = GetCargoDist(cargoContainers, CARGO_DESTINATION);
        if (cargoDestinations == null)
        {
            Print("Целевой контейнер не найден");
            UpdatePrint();
            return;
        }

        _isConnect = false;

        var cargoSources = GetCargoSource(cargoContainers, CARGO_SOURCE);
        if (cargoSources == null || cargoSources.Count == 0)
        {
            ShowCargoAmout(cargoDestinations);
            Print("Контейнер базы не найден");
            UpdatePrint();
            return;
        }

        _isConnect = true;

        //        ViewCargoInventory(cargoSources[0] as IMyCargoContainer);

        try
        {
            TransferCargo(cargoSources, cargoDestinations, STEEL_PLATE, MAX_AMOUT, STEEL_PLATE_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, COMPUTER, MAX_AMOUT, COMPUTER_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, DISPLAY, MAX_AMOUT, DISPLAY_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, CONSTRUCTION_COMPONENT, MAX_AMOUT, CONSTRUCTION_COMPONENT_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, INTERIOR_PLATE, MAX_AMOUT, INTERIOR_PLATE_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, THRUST_COMPONENTS, MAX_AMOUT, THRUST_COMPONENTS_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, BULLETPROOF_GLASS, MAX_AMOUT, BULLETPROOF_GLASS_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, DETECTOR_COMPONENTS, MAX_AMOUT, DETECTOR_COMPONENTS_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, GIRDER, MAX_AMOUT, GIRDER_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, GIR_GRAVITY_GENERATOR_COMPONENTS_DER, MAX_AMOUT,
                GIR_GRAVITY_GENERATOR_COMPONENTS_DER_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, LARGE_STEEL_TUBE, MAX_AMOUT, LARGE_STEEL_TUBE_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, MEDICAL_COMPONENTS, MAX_AMOUT, MEDICAL_COMPONENTS_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, METAL_GRID, MAX_AMOUT, METAL_GRID_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, MOTOR, MAX_AMOUT, MOTOR_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, POWER_CELL, MAX_AMOUT, POWER_CELL_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, RADIO_COMMUNICATION_COMPONENTS, MAX_AMOUT, RADIO_COMMUNICATION_COMPONENTS_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, SMALL_STEEL_TUBE, MAX_AMOUT, SMALL_STEEL_TUBE_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, REACTOR_COMPONENTS, MAX_AMOUT, REACTOR_COMPONENTS_AMOUNT);
            TransferCargo(cargoSources, cargoDestinations, SOLAR_CELL, MAX_AMOUT, SOLAR_CELL_AMOUNT);

            ShowCargoAmout(cargoDestinations);
        }
        catch (Exception e)
        {
            Print(e.Message);
        }

        UpdatePrint();
    }

    public void Save()
    { }

    #endregion

    #region Private methods

    private static int GetIdxItems(List<IMyInventoryItem> inventoryItems, string subtypeName)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].Content.SubtypeName == subtypeName)
            {
                return i;
            }
        }

        return -1;
    }

    private void TransferCargo(List<IMyTerminalBlock> cargoSources, List<IMyTerminalBlock> cargoDestination, string subtypeName,
        int transferAmount,
        int maxAmout)
    {
        if (cargoDestination == null || cargoDestination.Count == 0)
        {
            Print($"Контейнеры для материаллов не найден ");
            return;
        }

        if (cargoSources == null || cargoSources.Count == 0)
        {
            Print("Контейнеры с материалами не подключен");
            return;
        }

        foreach (var src in cargoSources)
        {
            IMyInventory inventorySrc = src.GetInventory();

            var invontorySrc = inventorySrc.GetItems();
            int idxSrc = GetIdxItems(invontorySrc, subtypeName);

            if (idxSrc < 0)
            {
                continue;
            }

            foreach (var dst in cargoDestination)
            {
                IMyInventory inventoryDest = dst.GetInventory();

                if (inventoryDest.IsFull)
                {
                    continue;
                }

                var invontoryDests = inventoryDest.GetItems();
                int idxDst = GetIdxItems(invontoryDests, subtypeName);

                if (idxDst < 0)
                {
                    if (maxAmout > 0)
                        inventorySrc.TransferItemTo(inventoryDest, idxSrc, null, null, transferAmount);
                }
                else
                {
                    int a = maxAmout - invontoryDests[idxDst].Amount.ToIntSafe();
                    if (a > 0)
                    {
                        inventorySrc.TransferItemTo(inventoryDest, idxSrc, null, null, a);
                    }
                    else
                    {
                        inventoryDest.TransferItemTo(inventorySrc, idxDst, null, null, Math.Abs(a));
                    }
                }
            }
        }
    }

    private void PrintForAllTerminals(string str, bool append = false)
    {
        List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
        GridTerminalSystem.GetBlocksOfType(textPanels);

        foreach (var textPanel in textPanels)
        {
            if (textPanel.CustomName.Contains(TEXT_PANEL))
            {
                textPanel.WritePublicText(str, append);
            }
        }
    }

    private void UpdatePrint()
    {
        _curSign = _curSign < 3 ? ++_curSign : 0;

        if (_isConnect)
            PrintForAllTerminals("cc " + DateTime.Now.ToString("T") + "  " + _signs[_curSign] + "\n");
        else
            PrintForAllTerminals(DateTime.Now.ToString("T") + "  " + _signs[_curSign] + "\n");

        if (MsgsBuffer.Length > 0)
        {
            PrintForAllTerminals(MsgsBuffer.ToString(), true);
        }

        if (MsgsBuffer.Length > MAX_SYMBOLS)
        {
            MsgsBuffer.Clear();
        }
    }

    private void Print(string str)
    {
        MsgsBuffer.AppendLine(str);
    }

    private List<IMyTerminalBlock> GetCargoDist(List<IMyTerminalBlock> listCargo, string name)
    {
        return listCargo.Where(cargo => cargo.CustomName.Contains(name)).ToList();
    }

    private List<IMyTerminalBlock> GetCargoSource(List<IMyTerminalBlock> listCargo, string name)
    {
        return listCargo.Where(item => item.CustomName.Contains(name)).ToList();
    }

    private void ViewCargoInventory(IMyCargoContainer cargo)
    {
        if (cargo == null)
        {
            Print("Контейнер не найден");
            return;
        }

        Print($"Контейнер: {cargo.CustomName}");
        IMyInventory inventory = cargo.GetInventory();

        if (inventory != null)
        {
            Print($"items: {inventory.GetItems().Count}");
            foreach (var item in inventory.GetItems())
            {
                Print(
                    $"{cargo.CustomName} \n\t\tN: {item.Content.SubtypeName}, \n\t\tI: {item.Content.SubtypeId}, \n\t\tT: {item.Content.TypeId}");
            }
        }
        else
        {
            Print("Конейнер пуст");
        }
    }

    private void ShowCargoAmout(List<IMyTerminalBlock> cargoList)
    {
        MsgsBuffer.Clear();

        if (cargoList == null)
        {
            Print("Контейнер не найден");
            return;
        }

        int steelPlate = 0;
        int thrustComponents = 0;
        int bulletproofGlass = 0;
        int computer = 0;
        int constructionComponent = 0;
        int detectorComponents = 0;
        int display = 0;
        int girder = 0;
        int girGravityGeneratorComponentsDer = 0;
        int interiorPlate = 0;
        int largeSteelTube = 0;
        int medicalComponents = 0;
        int metalGrid = 0;
        int motor = 0;
        int powerCell = 0;
        int radioCommunicationComponents = 0;
        int reactorComponents = 0;
        int smallSteelTube = 0;
        int solarCell = 0;

        foreach (var cargo in cargoList)
        {
            IMyInventory inventory = cargo.GetInventory();
            if (inventory != null)
            {
                foreach (var item in inventory.GetItems())
                {
                    switch (item.Content.SubtypeName)
                    {
                        case STEEL_PLATE:
                            steelPlate = item.Amount.ToIntSafe();
                            break;
                        case THRUST_COMPONENTS:
                            thrustComponents = item.Amount.ToIntSafe();
                            break;
                        case BULLETPROOF_GLASS:
                            bulletproofGlass = item.Amount.ToIntSafe();
                            break;
                        case COMPUTER:
                            computer = item.Amount.ToIntSafe();
                            break;
                        case CONSTRUCTION_COMPONENT:
                            constructionComponent = item.Amount.ToIntSafe();
                            break;
                        case DETECTOR_COMPONENTS:
                            detectorComponents = item.Amount.ToIntSafe();
                            break;
                        case DISPLAY:
                            display = item.Amount.ToIntSafe();
                            break;
                        case GIRDER:
                            girder = item.Amount.ToIntSafe();
                            break;
                        case GIR_GRAVITY_GENERATOR_COMPONENTS_DER:
                            girGravityGeneratorComponentsDer = item.Amount.ToIntSafe();
                            break;
                        case INTERIOR_PLATE:
                            interiorPlate = item.Amount.ToIntSafe();
                            break;
                        case LARGE_STEEL_TUBE:
                            largeSteelTube = item.Amount.ToIntSafe();
                            break;
                        case MEDICAL_COMPONENTS:
                            medicalComponents = item.Amount.ToIntSafe();
                            break;
                        case METAL_GRID:
                            metalGrid = item.Amount.ToIntSafe();
                            break;
                        case MOTOR:
                            motor = item.Amount.ToIntSafe();
                            break;
                        case POWER_CELL:
                            powerCell = item.Amount.ToIntSafe();
                            break;
                        case RADIO_COMMUNICATION_COMPONENTS:
                            radioCommunicationComponents = item.Amount.ToIntSafe();
                            break;
                        case REACTOR_COMPONENTS:
                            reactorComponents = item.Amount.ToIntSafe();
                            break;
                        case SMALL_STEEL_TUBE:
                            smallSteelTube = item.Amount.ToIntSafe();
                            break;
                        case SOLAR_CELL:
                            solarCell = item.Amount.ToIntSafe();
                            break;
                    }
                }
            }
        }

        Print($"Стальная пластина             {steelPlate}");
        Print($"Детали ускорителя             {thrustComponents}");
        Print($"Бронированное стекло          {bulletproofGlass}");
        Print($"Компьютер                     {computer}");
        Print($"Строительный компонент        {constructionComponent}");
        Print($"Компоненты детектора          {detectorComponents}");
        Print($"Экран                         {display}");
        Print($"Балка                         {girder}");
        Print($"Гравитационного генератора    {girGravityGeneratorComponentsDer}");
        Print($"Пластина                      {interiorPlate}");
        Print($"Большая стальная труба        {largeSteelTube}");
        Print($"Медицинские компоненты        {medicalComponents}");
        Print($"Металлическая решетка         {metalGrid}");
        Print($"Мотор                         {motor}");
        Print($"Аккумулятор                   {powerCell}");
        Print($"Комплектующие для радио-связи {radioCommunicationComponents}");
        Print($"Компоненты реактора           {reactorComponents}");
        Print($"Маленькая стальная трубка     {smallSteelTube}");
        Print($"Солнечная батарея             {solarCell}");
    }

    #endregion

    #region Settings

    //Максимальное колличество символов которое возможно вывести без очистки буфера сообщений
    private const int MAX_SYMBOLS = 200;

    //Часть имени целевого контейнера
    private const string CARGO_DESTINATION = "Контейнер сборщика";

    //Часть имени исходного контейнера
    private const string CARGO_SOURCE = "Контейнер с материаллами базы";

    //Текстовая панель
    private const string TEXT_PANEL = "Текстовая панель";

    #endregion 

    #region SubtypeName

    //Стальная пластина
    private const string STEEL_PLATE = "SteelPlate";

    //Детали ускорителя
    private const string THRUST_COMPONENTS = "Thrust";

    //Бронированное стекло
    private const string BULLETPROOF_GLASS = "BulletproofGlass";

    //Компьютер
    private const string COMPUTER = "Computer";

    //Строительный компонент
    private const string CONSTRUCTION_COMPONENT = "Construction";

    //Компоненты детектора
    private const string DETECTOR_COMPONENTS = "Detector";

    //Экран
    private const string DISPLAY = "Display";

    //Балка
    private const string GIRDER = "Girder";

    //Компоненты гравитационного генератора 
    private const string GIR_GRAVITY_GENERATOR_COMPONENTS_DER = "GravityGenerator";

    //Пластина
    private const string INTERIOR_PLATE = "InteriorPlate";

    //Большая стальная труба
    private const string LARGE_STEEL_TUBE = "LargeTube";

    //Медицинские компоненты
    private const string MEDICAL_COMPONENTS = "Medical";

    //Металлическая решетка
    private const string METAL_GRID = "MetalGrid";

    //Мотор
    private const string MOTOR = "Motor";

    //Аккумулятор
    private const string POWER_CELL = "PowerCell";

    //Комплектующие для радио-связи
    private const string RADIO_COMMUNICATION_COMPONENTS = "RadioCommunication";

    //Компоненты реактора
    private const string REACTOR_COMPONENTS = "Reactor";

    //Маленькая стальная трубка
    private const string SMALL_STEEL_TUBE = "SmallTube";

    //Солнечная батарея
    private const string SOLAR_CELL = "SolarCell";

    #endregion

    //==================================================================================
}
}