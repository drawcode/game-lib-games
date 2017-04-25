using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#region scratch

// ----------------------------------------------------------------------------
// SCRATCH

//go.transform.position = infinityPosition.WithY(0);

//go.ResetLocalPosition();

/*
void LoadLevelAssetByIndex(int indexItem) {

    GameObject go = AppContentAssets.LoadAssetLevelAssets(data.codeGamePart);
    go.name = StringUtil.Dashed(data.code, indexItem.ToString());
    go.transform.parent = transform;

    if (go != null) {

        GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

        if (part != null) {

            part.index = indexItem;
            Vector3 bounds = part.bounds;

            go.transform.position = go.transform.position.WithZ(
                (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);


        }
    } 
}
*/

/*


string assetCodeBlock1 = "block-rock-1";

for (int i = 0; i < data.lines.Count; i++) {

    GameObject go = AppContentAssets.LoadAssetLevelAssets(assetCodeBlock1);
    go.name = StringUtil.Dashed(data.code, "0", indexItem.ToString());
    go.transform.parent = transform;


    if (go != null) {
        GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

        part.index = indexItem;
        Vector3 bounds = part.bounds;

        go.transform.position = go.transform.position.WithZ(
            (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);
    }
}

*/

// Layer in blocks

// Layer in obstacles

// Layer in collectables

// 

/*
GameObject go = AppContentAssets.LoadAssetLevelAssets(data.codeGamePart);
go.name = StringUtil.Dashed(data.code, indexItem.ToString());
go.transform.parent = transform;

if (go != null) {

    GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

    if (part != null) {

        part.index = indexItem;
        Vector3 bounds = part.bounds;

        go.transform.position = go.transform.position.WithZ(
            (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);
    }
}
*/


// ADD ITEMS

// ADD ITEM COIN

/*

int randCoin = UnityEngine.Random.Range(1, 10);

if (randCoin < 2 && !clear && !used) {

    string itemCoin = "item-coin";

    GameObject goAssetItem = AppContentAssets.LoadAssetItems(itemCoin);

    if (goAssetItem == null) {
        Debug.Log("Asset not found items/" + itemCoin);
        continue;
    }

    goAssetItem.Hide();

    goAssetItem.transform.parent = goItem.transform;
    goAssetItem.transform.position = goItem.transform.position.WithX(0);
    goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(2f).WithZ(0);

    goAssetItem.Show();

    used = true;
}

// ADD ITEM SPECIAL

int randLetter = UnityEngine.Random.Range(1, 10);

if (randLetter < 2 && !clear && !used) {

    string itemLetter = "item-special-watermelon";

    GameObject goAssetItem = AppContentAssets.LoadAssetItems(itemLetter);

    if (goAssetItem == null) {
        Debug.Log("Asset not found items/" + itemLetter);
        continue;
    }

    goAssetItem.Hide();

    goAssetItem.transform.parent = goItem.transform;
    goAssetItem.transform.position = goItem.transform.position.WithX(0);
    goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(2f).WithZ(0);

    goAssetItem.Show();

    used = true;
}
*/


/*
// ADD PART BLOCK JUMP AND ASSETS FROM TEMPLATE

int randObstacleLow = UnityEngine.Random.Range(1, 30);

if (randObstacleLow < 5 && !clear && !used) {
    GameObject goAssetObstacleLow = AppContentAssets.LoadAssetLevelAssets(data.codeGameBlockLow);

    if (goAssetObstacleLow == null) {
        Debug.Log("Asset not found levelassets/" + data.codeGameBlockLow);
        continue;
    }

    goAssetObstacleLow.Hide();

    goAssetObstacleLow.transform.parent = goItem.transform;
    goAssetObstacleLow.transform.position = goItem.transform.position;

    //goAssetObstacleLow.transform.localPosition = goAssetObstacleLow.transform.localPosition.WithY(bounds.y);

    goAssetObstacleLow.Show();

    used = true;
}
*/

/*

void LoadLevelAssetDynamicByIndex(int indexItem, bool clear = false) {

    // Use off screen location to spawn before move
    Vector3 spawnLocation = Vector3.zero.WithY(5000);

    bool used = false;

    // ADD PART ITEMS CONTAINER

    GameObject go = AppContentAssets.LoadAssetLevelAssets(data.codeGamePartItems);

    if (go == null) {
        Debug.Log("Asset not found levelassets/" + data.codeGamePartItems);
        return;
    }

    go.DestroyChildren();

    go.name = StringUtil.Dashed(data.code, indexItem.ToString());
    go.transform.parent = transform;

    GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

    if (part == null) {
        Debug.Log("GameObjectInfinitePart not found part:" + data.codeGamePartItems);
        return;
    }

    part.index = indexItem;
    Vector3 bounds = part.bounds;

    //Vector3 infinityPosition = go.transform.position.WithZ(
    //    (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);

    go.transform.position = go.transform.position.WithZ(
        (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);

    // ADD PART ITEM CONTAINER AT LINES

    string template1 = "   ";
    string template2 = "X  ";
    string template3 = " X ";
    string template4 = "  X";

    for (int i = 0; i < data.lines.Count; i++) {

        GameObject goItem = AppContentAssets.LoadAssetLevelAssets(data.codeGamePartItem);

        if (goItem == null) {
            Debug.Log("Asset not found levelassets/" + data.codeGamePartItem);
            continue;
        }

        goItem.DestroyChildren();

        goItem.transform.parent = go.transform;
        goItem.transform.position = go.transform.position;
        goItem.transform.localPosition = goItem.transform.localPosition.WithX(data.lines[i].x);

        GameObjectInfinitePartItem partItem = goItem.Get<GameObjectInfinitePartItem>();

        if (partItem == null) {
            continue;
        }

        partItem.code = i.ToString();

        int rand = UnityEngine.Random.Range(1, 10);

        bool fillBlock = false;

        if (rand > 2 && !clear) {
            fillBlock = true;
        }
        else if (clear) {
            fillBlock = true;
        }

        if (fillBlock) {

            // ADD PART BLOCK AND ASSETS FROM TEMPLATE

            GameObject goAssetBlock = AppContentAssets.LoadAssetLevelAssets(data.codeGameBlock);

            if (goAssetBlock == null) {
                Debug.Log("Asset not found levelassets/" + data.codeGameBlock);
                continue;
            }

            goAssetBlock.Hide();

            goAssetBlock.transform.parent = goItem.transform;
            goAssetBlock.transform.position = goItem.transform.position;
            //goAssetBlock.transform.localPosition = goItem.transform.localPosition.WithX(data.lines[i].x);

            goAssetBlock.Show();
        }

        // ADD ITEMS

        // ADD ITEM COIN

        int randCoin = UnityEngine.Random.Range(1, 10);

        if (randCoin < 2 && !clear && !used) {

            string itemCoin = "item-coin";

            GameObject goAssetItem = AppContentAssets.LoadAssetItems(itemCoin);

            if (goAssetItem == null) {
                Debug.Log("Asset not found items/" + itemCoin);
                continue;
            }

            goAssetItem.Hide();

            goAssetItem.transform.parent = goItem.transform;
            goAssetItem.transform.position = goItem.transform.position.WithX(0);
            goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(2f).WithZ(0);

            goAssetItem.Show();

            used = true;
        }

        // ADD ITEM SPECIAL

        int randLetter = UnityEngine.Random.Range(1, 10);

        if (randLetter < 2 && !clear && !used) {

            string itemLetter = "item-special-watermelon";

            GameObject goAssetItem = AppContentAssets.LoadAssetItems(itemLetter);

            if (goAssetItem == null) {
                Debug.Log("Asset not found items/" + itemLetter);
                continue;
            }

            goAssetItem.Hide();

            goAssetItem.transform.parent = goItem.transform;
            goAssetItem.transform.position = goItem.transform.position.WithX(0);
            goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(2f).WithZ(0);

            goAssetItem.Show();

            used = true;
        }


        // ADD PART BLOCK JUMP AND ASSETS FROM TEMPLATE

        int randObstacleLow = UnityEngine.Random.Range(1, 30);

        if (randObstacleLow < 5 && !clear && !used) {
            GameObject goAssetObstacleLow = AppContentAssets.LoadAssetLevelAssets(data.codeGameBlockLow);

            if (goAssetObstacleLow == null) {
                Debug.Log("Asset not found levelassets/" + data.codeGameBlockLow);
                continue;
            }

            goAssetObstacleLow.Hide();

            goAssetObstacleLow.transform.parent = goItem.transform;
            goAssetObstacleLow.transform.position = goItem.transform.position;

            //goAssetObstacleLow.transform.localPosition = goAssetObstacleLow.transform.localPosition.WithY(bounds.y);

            goAssetObstacleLow.Show();

            used = true;
        }

    }

    //go.transform.position = infinityPosition.WithY(0);


    //go.ResetLocalPosition();

}

*/

#endregion