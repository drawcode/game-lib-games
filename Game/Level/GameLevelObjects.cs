using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class GameLevelItemDataType {
    public static string randomType = "random";
    public static string explicitType = "explicit";
}

public class GameLevelItemData {
    public string code = "rock-1";
    public int count = 1;
    public Vector3 pos = Vector3.zero;
    public string type = GameLevelItemDataType.randomType;

    public GameLevelItemData() {

    }

    public GameLevelItemData(string codeTo, string typeTo, int countTo, Vector3 posTo) {
        code = codeTo;
        type = typeTo;
        count = countTo;
        pos = posTo;
    }
}

public class GameLevelTemplate {
    public Dictionary<string, GameLevelItemData> randomAssets = new Dictionary<string, GameLevelItemData>();
    public Dictionary<string, Vector3> placedAssets = new Dictionary<string, Vector3>();
}

public class GameLevelGridData {

    public float gridHeight;
    public float gridWidth;
    public float gridDepth;
    public float gridBoxSize;

    public bool centeredX;
    public bool centeredY;
    public bool centeredZ;

    public List<string> presets;
    public List<AppContentAsset> assets;

    //public string[,,] assetMap;

    public Dictionary<string, GameLevelItemAssetData> assetLayoutData;

    public GameLevelGridData() {

        presets = new List<string>();

        //gridHeight = (float)GameLevels.currentLevelData.grid_height;
        //gridWidth = (float)GameLevels.currentLevelData.grid_width;
        //gridDepth = (float)GameLevels.currentLevelData.grid_depth;
        //gridBoxSize = (float)GameLevels.currentLevelData.grid_box_size;

        //centeredX = GameLevels.currentLevelData.grid_centered_x;
        //centeredY = GameLevels.currentLevelData.grid_centered_y;
        //centeredZ = GameLevels.currentLevelData.grid_centered_z;

        gridHeight = (float)GameLevels.Current.data.level_data.grid_height;
        gridWidth = (float)GameLevels.Current.data.level_data.grid_width;
        gridDepth = (float)GameLevels.Current.data.level_data.grid_depth;
        gridBoxSize = (float)GameLevels.Current.data.level_data.grid_box_size;

        centeredX = GameLevels.Current.data.level_data.grid_centered_x;
        centeredY = GameLevels.Current.data.level_data.grid_centered_y;
        centeredZ = GameLevels.Current.data.level_data.grid_centered_z;

        // GameLevels.Current.data

        Reset();
    }

    public void Reset() {
        ResetGrid((int)gridHeight, (int)gridWidth, (int)gridDepth);
        ClearAssets();
        ClearMap();
        ClearPresets();
    }

    public void ResetGrid(int height, int width, int depth) {
        ResetGrid(height, width, depth, (int)gridBoxSize, true, false, true);
    }

    public void ResetGrid(int height, int width, int depth, int boxSize, bool centerX, bool centerY, bool centerZ) {
        gridHeight = (float)height;
        gridWidth = (float)width;
        gridDepth = (float)depth;
        gridBoxSize = (float)boxSize;

        centeredX = centerX;
        centeredY = centerY;
        centeredZ = centerZ;
    }

    public static GameLevelGridData GetLevelTemplate(string template) {

        //string wall1 = "wall-1";

        //GameLevelGridData gridData = new GameLevelGridData();


        return GameLevels.GetLevelGridBaseDefault();

        /*
        GameLevelGridData data = new GameLevelGridData();
        data = AddAssets(data, "bush-1", UnityEngine.Random.Range(4,8));
        data = AddAssets(data, "box-1", UnityEngine.Random.Range(2, 4));
        data = AddAssets(data, "padding-1", UnityEngine.Random.Range(1, 3));

        return data;
        */
    }

    public static GameLevelGridData GetBaseDefault() {

        return GameLevels.GetLevelGridBaseDefault();

        /*
        GameLevelGridData data = new GameLevelGridData();
        data = AddAssets(data, "bush-1", UnityEngine.Random.Range(4,8));
        data = AddAssets(data, "box-1", UnityEngine.Random.Range(2, 4));
        data = AddAssets(data, "padding-1", UnityEngine.Random.Range(1, 3));

        return data;
        */
    }

    public static GameLevelGridData GetDefault() {

        GameLevelGridData data = GameLevelGridData.GetBaseDefault();
        data.RandomizeAssetsInAssetMap();

        return data;
    }

    public static GameLevelGridData GetTemplate(string templateCode) {

        GameLevelGridData data = GameLevelGridData.GetLevelTemplate(templateCode);
        //data.RandomizeAssetsInAssetMap();

        return data;
    }

    public static GameLevelGridData GetModeTypeChoice(int choiceCount) {

        GameLevelGridData data = GameLevelGridData.GetBaseDefault();
        //data = AddAssets(data, "game-choice-item", choiceCount);
        data.RandomizeAssetsInAssetMap();

        return data;
    }

    public static GameLevelGridData AddAssets(GameLevelGridData data, string assetCode) {
        data.SetAssets(assetCode, 1);
        return data;
    }

    public static GameLevelGridData AddAssets(GameLevelGridData data, string assetCode, int count) {
        data.SetAssets(assetCode, count);
        return data;
    }

    //public string[,,] GetAssetMap() {
    //    return assetMap;
    //}

    public Dictionary<string, GameLevelItemAssetData> GetAssetLayoutData() {
        return assetLayoutData;
    }

    public void ClearAssets() {
        assets = new List<AppContentAsset>();
    }

    public void ClearMap() {
        //assetMap = new string[(int)gridWidth, (int)gridHeight, (int)gridDepth];
        assetLayoutData = new Dictionary<string, GameLevelItemAssetData>();
    }

    public void ClearPresets() {
        presets = new List<string>();
    }

    public void SetAssets(string code, int count) {
        for(int i = 0; i < count; i++) {
            SetAsset(code);
        }
    }

    public void SetAsset(string code) {
        AppContentAsset asset = AppContentAssets.Instance.GetByCode(code);
        if(asset != null) {
            //if(!HasAsset(asset.code)) {
            assets.Add(asset);
            //}
        }
    }

    public bool HasAsset(string code) {
        foreach(AppContentAsset asset in assets) {
            if(asset.code == code) {
                return true;
            }
        }

        return false;
    }

    //public void SetAssetsInAssetMap(
    //    string code, string type, string dataType, string displayType, Vector3 pos) {

    //    GameLevelItemAssetData assetData = new GameLevelItemAssetData();

    //    assetData.code = code;
    //    assetData.type = type;
    //    assetData.data_type = dataType;
    //    assetData.display_type = displayType;
    //    assetData.position_data = new Vector3Data(pos);
    //    assetData.SetAssetScaleRange(.7f, 1.2f);
    //    assetData.SetAssetRotationRangeY(-180, 180);

    //    SetAssetsInAssetMap(assetData);
    //}

    public void SetAssetsInAssetMap(
        string code, string type, string dataType, string displayType,
        Vector3 pos, Vector3 scale, Vector3 rotation) {

        GameLevelItemAssetData assetData = new GameLevelItemAssetData();

        assetData.code = code;
        assetData.type = type;
        assetData.data_type = dataType;
        assetData.display_type = displayType;
        assetData.position_data = new Vector3Data(pos);
        assetData.scale_data = new Vector3Data(scale);
        assetData.rotation_data = new Vector3Data(rotation);

        SetAssetsInAssetMap(assetData);
    }

    public void SetAssetsInAssetMap(
        string code, string type, string dataType, string displayType,
        Vector3 pos, Vector3 scale, Vector3 rotation, Vector3 localPosition) {

        GameLevelItemAssetData assetData = new GameLevelItemAssetData();

        assetData.code = code;
        assetData.type = type;
        assetData.data_type = dataType;
        assetData.display_type = displayType;
        assetData.position_data = new Vector3Data(pos);
        assetData.scale_data = new Vector3Data(scale);
        assetData.rotation_data = new Vector3Data(rotation);
        assetData.local_position_data = new Vector3Data(localPosition);

        SetAssetsInAssetMap(assetData);
    }

    public void SetAssetsInAssetMap(GameLevelItemAssetData assetData) {

        Vector3 pos = assetData.position_data.GetVector3();

        if(pos.x > gridWidth - 1) {
            pos.x = gridWidth - 1;
        }

        if(pos.y > gridHeight - 1) {
            pos.y = gridHeight - 1;
        }

        if(pos.z > gridDepth - 1) {
            pos.z = gridDepth - 1;
        }

        string keyLayout =
            string.Format(
                "{0}-{1}-{2}",
                (int)pos.x,
                (int)pos.y,
                (int)pos.z);

        assetData.position_data.FromVector3(pos);

        if(!assetLayoutData.ContainsKey(keyLayout)) {

            if(assetData.code != BaseDataObjectKeys.empty) {

                if(assetData.type == BaseDataObjectKeys.character) {

                    Debug.Log("SetAssetsIntoMap:keyLayout:" + keyLayout);
                    Debug.Log("SetAssetsIntoMap:assetData:" + assetData.ToJson());
                }
            }

            assetLayoutData.Set(keyLayout, assetData);
        }
    }

    // data.rangeScale = Vector3.zero.WithX(.7f).WithY(1.2f);
    // data.range_rotation = Vector3.zero.WithX(-180).WithY(180); 

    public void RandomizeAssetsInAssetMap() {

        foreach(AppContentAsset asset in assets) {

            int x = 0;
            int y = 0;
            int z = 0;

            x = UnityEngine.Random.Range(0, (int)gridWidth - 1);
            y = UnityEngine.Random.Range(0, (int)gridHeight - 1);
            z = UnityEngine.Random.Range(0, (int)gridDepth - 1);

            int midX = ((int)((gridWidth - 1) / 2));
            // TODO 2d version
            //int midY = ((int)((gridHeight - 1) / 2));
            int midZ = ((int)((gridDepth - 1) / 2));

            // Dont' add if in the middle spawn area until player
            // items grid out in level data.
            // TODO switch to area around player to gid out items
            // if spawns on level items

            if((x < (midX + 2)) && (x > (midX - 2))
                && (z < (midZ + 2)) && (z > (midZ - 2))) {
                continue;
            }

            string keyLayout = string.Format("{0}-{1}-{2}", x, y, z);

            if(!assetLayoutData.ContainsKey(keyLayout)) {

                Vector3 posData = Vector3.one.WithX(x).WithY(y).WithZ(z);

                //

                Vector3 scaleData = MathUtil.RandomRangeConstrain(
                    .7f, 1.3f);
                
                if(asset.ContainsKey(BaseDataObjectKeys.scale_min)
                    && asset.ContainsKey(BaseDataObjectKeys.scale_max)) {

                    double scaleMin = asset.scale_min;
                    double scaleMax = asset.scale_max;

                    scaleData = MathUtil.RandomRangeConstrain(
                        (float)scaleMin, (float)scaleMax);

                }
                else if(asset.ContainsKey(BaseDataObjectKeys.scale_data_min)
                    && asset.ContainsKey(BaseDataObjectKeys.scale_data_max)) {

                    Vector3Data scaleDataMin = asset.scale_data_min;
                    Vector3Data scaleDataMax = asset.scale_data_max;

                    if(scaleDataMin != null && scaleDataMax != null) {

                        scaleData = MathUtil.RandomRangeConstrain(
                            scaleDataMin.GetVector3().x, scaleDataMax.GetVector3().x);

                        //if(scaleDataMax.x == 1) {
                        //    scaleData.x = 1;
                        //}

                        //if(scaleDataMax.y == 1) {
                        //    scaleData.y = 1;
                        //}
                        //if(scaleDataMax.z == 1) {
                        //    scaleData.z = 1;
                        //}
                    }
                }

                Vector3 rotationData = Vector3.zero.WithY(
                    MathUtil.RandomRangeY(-180, 180).y);

                if(asset.ContainsKey(BaseDataObjectKeys.rotation_min)
                    && asset.ContainsKey(BaseDataObjectKeys.rotation_max)) {

                    double rotationMin = asset.rotation_min;
                    double rotationMax = asset.rotation_max;

                    rotationData = MathUtil.RandomRangeConstrain(
                        (float)rotationMin, (float)rotationMax);

                }
                else if(asset.ContainsKey(BaseDataObjectKeys.scale_data_min)
                    && asset.ContainsKey(BaseDataObjectKeys.scale_data_max)) {


                    Vector3Data rotationDataMin = asset.rotation_data_min;
                    Vector3Data rotationDataMax = asset.rotation_data_max;

                    if(rotationDataMin != null && rotationDataMax != null) {

                        rotationData = MathUtil.RandomRangeConstrain(
                            rotationDataMin.GetVector3(), rotationDataMax.GetVector3());

                        // TODO rotation 

                        //if(rotationDataMax.x == 0) {
                        //    rotationData.x = 0;
                        //}

                        //if(rotationDataMax.y == 0) {
                        //    rotationData.y = 0;
                        //}

                        //if(rotationDataMax.z == 0) {
                        //    rotationData.z = 0;
                        //}
                    }
                }

                //assetData.SetAssetScaleRange(.7f, 1.2f);
                //assetData.SetAssetRotationRangeY(-180, 180);

                SetAssetsInAssetMap(
                    asset.code, asset.type, asset.data_type, asset.display_type,
                    posData, scaleData, rotationData);
            }
        }
    }
}