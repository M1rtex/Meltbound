using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class HealthBarTests
{
    private GameObject gameGameObject;
    private HealthBar healthBar;
    private Slider slider;
    private GameObject restartUI;

    [SetUp]
    public void Setup()
    {
        gameGameObject = new GameObject();
        GameObject sliderObj = new GameObject();
        slider = sliderObj.AddComponent<Slider>();
        
        restartUI = new GameObject();
        restartUI.SetActive(false);
        
        healthBar = gameGameObject.AddComponent<HealthBar>();
        healthBar.slider = slider;
        healthBar.RestartUI = restartUI;
        healthBar.maxHealth = 100f;
        healthBar.decayRate = 10f; 
        Time.timeScale = 1f;
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(gameGameObject);
        Object.Destroy(slider.gameObject);
        Object.Destroy(restartUI);
        Time.timeScale = 1f;
    }

    [UnityTest]
    public IEnumerator HealthBar_Starts_WithFullHealth()
    {
        yield return null;
        Assert.AreEqual(healthBar.maxHealth, slider.value, 1f);
    }

    [UnityTest]
    public IEnumerator Heal_IncreasesHealth_AndClampsToMax()
    {
        yield return null;
        healthBar.Heal(-20f); 
        Assert.AreEqual(80f, slider.value, 1f);

        healthBar.Heal(50f);
        Assert.AreEqual(100f, slider.value, 1f, "Здоровье не должно превышать maxHealth");
    }

    [UnityTest]
    public IEnumerator Health_Decays_OverTime()
    {
        float initialValue = slider.value;
        yield return new WaitForSeconds(1f);

        Assert.Less(slider.value, initialValue, "Здоровье должно уменьшаться в Update");
    }

    [UnityTest]
    public IEnumerator HealthHitsZero_TriggersGameOver()
    {
        healthBar.decayRate = 1000f; 

        yield return new WaitForSecondsRealtime(0.2f);

        Assert.AreEqual(0, slider.value, 1f);
        Assert.IsTrue(restartUI.activeSelf, "Экран RestartUI должен быть активен");
        Assert.AreEqual(0f, Time.timeScale, 1f, "Time.timeScale должен стать 0");
    }
}

// public class ItemHealTests
// {
//     private GameObject player;
//     private HealthBar healthBar;
//     private GameObject item;
//     private ItemHeal itemHeal;
//
//     [SetUp]
//     public void Setup()
//     {
//         player = new GameObject("Player");
//         player.tag = "Player";
//         
//         player.AddComponent<Rigidbody2D>().isKinematic = true;
//         player.AddComponent<BoxCollider2D>().isTrigger = true;
//         
//         healthBar = player.AddComponent<HealthBar>();
//         GameObject canvas = new GameObject("Canvas");
//         healthBar.slider = canvas.AddComponent<Slider>();
//         healthBar.RestartUI = new GameObject("RestartUI");
//         healthBar.maxHealth = 100f;
//         
//         item = new GameObject("HealItem");
//         item.AddComponent<BoxCollider2D>().isTrigger = true;
//         itemHeal = item.AddComponent<ItemHeal>();
//         itemHeal.healAmount = 20f;
//
//         Time.timeScale = 1f;
//     }
//
//     [TearDown]
//     public void Teardown()
//     {
//         if (player != null) 
//         {
//             Object.Destroy(player);
//         }
//
//         if (item != null) 
//         {
//             Object.Destroy(item);
//         }
//         Time.timeScale = 1f;
//         GameObject canvas = GameObject.Find("Canvas");
//         if (canvas != null) 
//         {
//             Object.Destroy(canvas);
//         }
//     }
//
//     [UnityTest]
//     public IEnumerator Item_HealsPlayer_OnCollision()
//     {
//         yield return null; // Ждем Start()
//         healthBar.Heal(-50f); 
//         float healthBefore = 50f;
//         item.transform.position = player.transform.position;
//         yield return new WaitForFixedUpdate();
//         Assert.AreEqual(healthBefore + itemHeal.healAmount, healthBar.slider.value, 1f);
//     }
//
//     [UnityTest]
//     public IEnumerator Item_IsDestroyed_AfterPickup()
//     {
//         // 1. Запоминаем ссылку на объект аптечки
//         GameObject itemReference = item; 
//
//         // 2. Перемещаем аптечку на игрока
//         itemReference.transform.position = player.transform.position;
//
//         // 3. Ждем обработки физики (FixedUpdate)
//         yield return new WaitForFixedUpdate();
//     
//         // 4. Ждем завершения кадра, чтобы Unity успела выполнить Destroy()
//         yield return null; 
//
//         // КРИТИЧЕСКИЙ МОМЕНТ: 
//         // Теперь мы НЕ обращаемся к itemReference.transform. 
//         // Мы просто проверяем, пустая ли ссылка.
//         Assert.That(itemReference, Is.Null, "Аптечка должна быть уничтожена после подбора");
//     }
//
//     [UnityTest]
//     public IEnumerator Item_DoesNotHeal_UntaggedObject()
//     {
//         player.tag = "Untagged"; 
//         healthBar.Heal(-50f);
//
//         item.transform.position = player.transform.position;
//         yield return new WaitForFixedUpdate();
//
//         Assert.AreEqual(50f, healthBar.slider.value, 0.1f, "Здоровье не должно измениться без тега Player");
//         Assert.IsFalse(item == null, "Аптечка не должна уничтожаться при контакте с не-игроком");
//     }
// }