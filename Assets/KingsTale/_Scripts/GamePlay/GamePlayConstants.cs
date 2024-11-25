using UnityEngine;

public class GamePlayConstants
{
    [Header("Tags")]
    public const string BUILDING_TAG = "Building";
    
    [Header("Input")]
    public const string ACT_MOVE_CAM = "MoveCamera";
    public const string ACT_MOVE_MOUSE = "MoveMouse";
    public const string ACT_ROTATE_CAM = "RotateCamera";
    public const string ACT_LEFT_CLICK = "LeftClick";
    public const string ACT_MIDDLE_CLICK= "MiddleClick";
    public const string ACT_RIGHT_CLICK = "RightClick";
    public const string ACT_LONG_PRESS = "LongPress";
    public const string ACT_ZOOM = "Zoom";
    public const string ACT_SHOP = "Shop";
    public const string ACT_MAP = "Map";
    public const string ACT_ESCAPE = "Escape";
    public const string ACT_POINTER_POS = "PointerPos";
    public const string ACT_NUMPAD = "Numpad";
    public const string ACT_BUILD_ARMY = "BuildArmy";
    
    [Header("Animator")]
    public const string VELOCITY_ANIMATOR_PAR = "Velocity";
    public const string DIE_ANIMATOR_PAR = "Die";
    public const string ATTACK_ANIMATOR_PAR = "Attack";
    public const string WORK_ANIMATOR_PAR = "Work";
    public const string MINE_ANIMATOR_PAR = "Mine";
    public const string RUN_ANIMATOR_PAR = "Run";

    [Header("Options")]
    public const uint HOUSE_PLACES_COUNT = 15;
}