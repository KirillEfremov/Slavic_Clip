/// <summary>
/// Weapon.cs
/// Author: MutantGopher
/// This is the core script that is used to create weapons.  There are 3 basic
/// types of weapons that can be made with this script:
/// 
/// Raycast - Uses raycasting to make instant hits where the weapon is pointed starting at
/// the position of raycastStartSpot
/// 
/// Projectile - Instantiates projectiles and lets them handle things like damage and accuracy.
/// 
/// Beam - Uses line renderers to create a beam effect.  This applies damage and force on 
/// every frame in small amounts, rather than all at once.  The beam type is limited by a
/// heat variable (similar to ammo for raycast and projectile) unless otherwise specified.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Mirror;

namespace Builds
{
	public enum WeaponType
	{
		Projectile,
		Raycast,
		Beam
	}
	public enum Auto
	{
		Full,
		Semi
	}
	public enum BulletHoleSystem
	{
		Tag,
		Material,
		Physic_Material
	}


	[System.Serializable]
	public class SmartBulletHoleGroup
	{
		public string tag;
		public Material material;
		public PhysicMaterial physicMaterial;
		public BulletHolePool bulletHole;

		public SmartBulletHoleGroup()
		{
			tag = "Everything";
			material = null;
			physicMaterial = null;
			bulletHole = null;
		}
		public SmartBulletHoleGroup(string t, Material m, PhysicMaterial pm, BulletHolePool bh)
		{
			tag = t;
			material = m;
			physicMaterial = pm;
			bulletHole = bh;
		}
	}

    // Класс оружия сам управляет механикой оружия
    public class Weapon : NetworkBehaviour
	{
		// Тип оружия
		public WeaponType type = WeaponType.Projectile;     // Какую систему вооружения следует использовать

        // Внешние инструменты
        public bool shooterAIEnabled = false;               // Включите функции, совместимые с Shooter AI от Gateway Games
        public bool bloodyMessEnabled = false;              // Включите функции, совместимые с Bloody Mess от Heavy Diesel Softworks
        public int weaponType = 0;                          // Собственность кровавого месива

        // Автоматическая стрельба
        public Auto auto = Auto.Full;                       // Как стреляет это оружие - полуавтоматически или полностью автоматически

        // Общие
        public bool playerWeapon = true;                    // Независимо от того, является ли это оружием игрока или нет, в отличие от оружия искусственного интеллекта
        public GameObject weaponModel;                      // Фактическая сетка для этого оружия
        public Transform raycastStartSpot;                  // Точка, которую оружейная система, использующая луч, должна использовать в качестве отправной точки для лучаs
        public float delayBeforeFire = 0.0f;                // Необязательная задержка, которая заставляет оружие выстрелить на заданный промежуток времени позже, чем обычно (0 для отсутствия задержки)

        // Зарядка
        public bool warmup = false;                         // Будет ли разрешено выстрелу "прогреться" перед выстрелом или нет - позволяет увеличить мощность, если игрок дольше удерживает нажатой кнопку "огонь".
                                                            // Работает только на полуавтоматическом лучевом и метательном оружии
        public float maxWarmup = 2.0f;                      // Максимальное количество времени, в течение которого прогрев может оказать какое-либо влияние на мощность и т.д.
        public bool multiplyForce = true;                   // Следует ли умножать начальную силу удара снаряда на величину теплоты прогрева - только для снарядов
        public bool multiplyPower = false;                  // Следует ли умножать урон от снаряда на величину теплоты прогрева - только для снарядов
                                                            // Также действует только на снаряды, использующие систему прямого урона - примером может служить стрела, которая наноси больше урона, чем дольше вы натягиваете лук
        public float powerMultiplier = 1.0f;                // Множитель, с помощью которого прогрев может повлиять на мощность оружия; мощность = мощность * (тепло * множитель мощности)
        public float initialForceMultiplier = 1.0f;         // Множитель, с помощью которого разминка может повлиять на начальную силу, предполагающую систему снарядов
        public bool allowCancel = false;                    // Если значение true, игрок может отменить этот прогретый выстрел, нажав кнопку ввода "Отмена", в противном случае выстрел будет произведен, когда игрок отпустит клавишу fire
        private float heat = 0.0f;                          // Количество времени, в течение которого оружие прогревалось, может находиться в диапазоне (0, максимальный прогрев)

        // Снаряд
        public GameObject projectile;                       // Снаряд, который будет запущен (если тип - снаряд)
        public Transform projectileSpawnSpot;               // Место, где должен быть создан экземпляр снаряда

        // Луч
        public bool reflect = true;                         // Должен ли лазерный луч отражаться от определенных поверхностей или нет
        public Material reflectionMaterial;                 // Материал, который отражает лазер. Если это значение равно нулю, лазер будет отражаться от всех поверхностей
        public int maxReflections = 2;                      // Максимальное количество раз, которое лазерный луч может отразиться от поверхности. Без этого ограничения система может застрять в бесконечном цикле
        public string beamTypeName = "laser_beam";          // Это имя, которое будет использоваться в качестве имени созданного эффекта луча. В этом нет необходимости.
        public float maxBeamHeat = 1.0f;                    // Время, в секундах, в течение которого будет действовать луч
        public bool infiniteBeam = false;                   // Если это так, то луч никогда не перегреется (эквивалентно бесконечному запасу боеприпасов).
        public Material beamMaterial;                       // Материал, который будет использоваться в луче (средство визуализации линий.) Это должен быть материал частиц
        public Color beamColor = Color.red;                 // Цвет, который будет использоваться для окрашивания материала луча
        public float startBeamWidth = 0.5f;                 // Ширина луча на исходной стороне
        public float endBeamWidth = 1.0f;                   // Ширина луча с торцевой стороны
        private float beamHeat = 0.0f;                      // Таймер для отслеживания прогрева и восстановления луча
        private bool coolingDown = false;                   // Независимо от того, остывает лучевое оружие в данный момент или нет. Это используется для того, чтобы убедиться, что оружие не сработает, когда оно слишком близко к максимальному уровню нагрева
        private GameObject beamGO;                          // Ссылка на созданный игровой объект луч
        private bool beaming = false;                       // Независимо от того, стреляет ли оружие в данный момент лучом или нет - используется, чтобы убедиться, что функция Stop Beam() вызывается после того, как луч больше не запускается

        // Мощность
        public float power = 80.0f;                         // Количество энергии, которой обладает это оружие (какой урон оно может нанести) (если тип - лучевой или лучевой луч)
        public float forceMultiplier = 10.0f;               // Множитель, используемый для изменения величины усилия, прикладываемого к твердым телам, которые выстреливаются
        public float beamPower = 1.0f;                      // Используется для определения урона, наносимого лучевым оружием. Это значение будет намного ниже, поскольку оно применяется к цели в каждом кадре при стрельбе

        // Диапазон
        public float range = 9999.0f;                       // Как далеко может стрелять это оружие (для raycast и beam)

        // Скорострельность
        public float rateOfFire = 10;                       // Количество выстрелов, производимых этим оружием в секунду
        private float actualROF;                            // Частота между выстрелами зависит от скорострельности
        private float fireTimer;                            // Таймер, используемый для срабатывания с заданной частотой

        // Боеприпасы
        public bool infiniteAmmo = false;                   // Независимо от того, должно ли это оружие иметь неограниченный боезапас или нет
        public int ammoCapacity = 12;                       // Количество выстрелов, которые может произвести это оружие, прежде чем его придется перезаряжать
        public int shotPerRound = 1;                        // Количество "пуль", которые будут выпущены в каждом раунде. Обычно это значение равно 1, но устанавливается на большее число для таких вещей, как дробовики с разбросом
        private int currentAmmo;                            // Сколько боеприпасов в данный момент имеется в оружии
        public float reloadTime = 2.0f;                     // Сколько времени требуется на перезарядку оружия
        public bool showCurrentAmmo = true;                 // Следует ли отображать текущие боеприпасы в графическом интерфейсе или нет
        public bool reloadAutomatically = true;             // Должно ли оружие автоматически перезаряжаться, когда заканчиваются патроны, или нет

        // Точность
        public float accuracy = 90.0f;                      // Насколько точно это оружие по шкале от 0 до 100
        private float currentAccuracy;                      // Сохраняет текущую точность. Используется для изменения точности в зависимости от скорости и т.д.
        public float accuracyDropPerShot = 1.0f;            // Насколько точность будет снижаться при каждом выстреле
        public float accuracyRecoverRate = 0.1f;            // Как быстро восстанавливается точность после каждого выстрела (значение от 0 до 1)

        // Взрыв
        public int burstRate = 3;                           // Количество выстрелов, произведенных в каждой очереди
        public float burstPause = 0.0f;                     // Время паузы между очередями
        private int burstCounter = 0;                       // Счетчик для отслеживания того, сколько выстрелов было произведено за одну серию
        private float burstTimer = 0.0f;                    // Таймер для отслеживания того, как долго оружие выдерживало паузу между очередями

        // Отдача
        public bool recoil = true;                          // Должна ли у этого оружия быть отдача или нет
        public float recoilKickBackMin = 0.1f;              // Минимальное расстояние, на которое оружие отскакивает назад при выстреле
        public float recoilKickBackMax = 0.3f;              // Максимальное расстояние, на которое оружие отскакивает назад при выстреле
        public float recoilRotationMin = 0.1f;              // Минимальное вращение, при котором оружие сработает при выстреле
        public float recoilRotationMax = 0.25f;             // Максимальное вращение, при котором оружие совершает удар при выстреле
        public float recoilRecoveryRate = 0.01f;            // Скорость, с которой оружие восстанавливается после смещения отдачи

        // Эффекты
        public bool spitShells = false;                     // Должно ли это оружие выбрасывать снаряды в сторону или нет
        public GameObject shell;                            // Снаряд, побуждающий выплюнуть боковую часть оружия
        public float shellSpitForce = 1.0f;                 // Сила, с которой снаряды будут выбрасываться из оружия
        public float shellForceRandom = 0.5f;               // Вариант, при котором сила плевка может изменяться + или - для каждого выстрела
        public float shellSpitTorqueX = 0.0f;               // Крутящий момент, с которым корпуса будут вращаться вокруг оси x
        public float shellSpitTorqueY = 0.0f;               // Крутящий момент, с которым корпуса будут вращаться вокруг оси y
        public float shellTorqueRandom = 1.0f;              // Вариант, при котором крутящий момент вращения может изменяться + или - для каждого выстрела
        public Transform shellSpitPosition;                 // Место, откуда оружие должно выбрасывать снаряды
        public bool makeMuzzleEffects = true;               // Должно ли оружие создавать дульный эффект или нет
        public GameObject[] muzzleEffects =
			new GameObject[] { null };                      // Эффекты, которые будут появляться на дуле пистолета (дульная вспышка, дым и т.д.)
        public Transform muzzleEffectsPosition;             // Место, откуда должны появляться дульные эффекты
        public bool makeHitEffects = true;                  // Независимо от того, должно ли оружие производить этот эффект или нет
        public GameObject[] hitEffects =
			new GameObject[] { null };                      // Эффекты, которые будут отображаться в том месте, куда попала "пуля"

        // Пулевые отверстия
        public bool makeBulletHoles = true;                 // Следует ли делать отверстия от пуль или нет
        public BulletHoleSystem bhSystem = BulletHoleSystem.Tag;    // На каком условии должны основываться динамические пулевые отверстия
        public List<string> bulletHolePoolNames = new
			List<string>();                                 // Список строк, содержащих названия пулевых отверстий в сцене
        public List<string> defaultBulletHolePoolNames =
			new List<string>();                             // Список строк, содержащих названия пулов пулевых отверстий по умолчанию в сцене
        public List<SmartBulletHoleGroup> bulletHoleGroups =
			new List<SmartBulletHoleGroup>();               // Список групп пулевых отверстий. В каждой из них есть метка для игровых объектов, в которые можно попасть, а также соответствующее пулевое отверстие
        public List<BulletHolePool> defaultBulletHoles =
			new List<BulletHolePool>();                     // Список маркированных отверстий по умолчанию, которые будут созданы при выполнении ни одного из пользовательских параметров
        public List<SmartBulletHoleGroup> bulletHoleExceptions =
			new List<SmartBulletHoleGroup>();               // Список объектов группы интеллектуальных пулевых отверстий, определяющий условия, при которых не будет создан экземпляр пулевого отверстия.
                                                            // Другими словами, пулевые отверстия в списке пулевых отверстий по умолчанию будут созданы на любой поверхности, за исключением
                                                            // те, что указаны в этом списке.

        // Перекрестие прицела
        public bool showCrosshair = true;                   // Должно ли отображаться перекрестие или нет
        public Texture2D crosshairTexture;                  // Текстура, используемая для рисования перекрестия
        public int crosshairLength = 10;                    // Длина каждой линии перекрестия
        public int crosshairWidth = 4;                      // Ширина каждой линии перекрестия
        public float startingCrosshairSize = 10.0f;         // Промежуток (в пикселях) между линиями перекрестия (для неточности оружия)
        private float currentCrosshairSize;                 // Промежуток между линиями перекрестия, который обновляется в зависимости от точности оружия в режиме реального времени

        // Аудио
        public AudioClip fireSound;                         // Звук, воспроизводимый при выстреле из оружия
        public AudioClip reloadSound;                       // Звук, воспроизводимый при перезарядке оружия
        public AudioClip dryFireSound;                      // Звук воспроизводится, когда пользователь пытается выстрелить, но у него заканчиваются патроны

        // Другое
        private bool canFire = true;                        // Может ли оружие в данный момент стрелять или нет (используется для полуавтоматического оружия)


        // Используйте это для инициализации
        void Start()
		{
			if (!transform.parent.parent.GetComponent<NetworkIdentity>().isLocalPlayer)
			{
                this.GetComponent<Weapon>().enabled = false;
            }
            // Рассчитайте фактическую КРЫШУ, которая будет использоваться в системах вооружения. Переменная скорострельности равна
            // разработанный для облегчения работы пользователя - он отображает количество выстрелов, которые необходимо произвести
            // в секунду. Здесь вычисляется фактическое десятичное значение ROF, которое можно использовать с таймерами.
            if (rateOfFire != 0)
				actualROF = 1.0f / rateOfFire;
			else
				actualROF = 0.01f;

            // Инициализируйте текущую переменную размера перекрестия начальным значением, указанным пользователем
            currentCrosshairSize = startingCrosshairSize;

            // Убедитесь, что таймер включения начинается с 0
            fireTimer = 0.0f;

            // Заряжайте оружие с полным магазином
            currentAmmo = ammoCapacity;

            // Снабдите это оружие компонентом источника звука, если у него его еще нет
            if (GetComponent<AudioSource>() == null)
			{
				gameObject.AddComponent(typeof(AudioSource));
			}

            // Убедитесь, что начальное значение raycast не равно null
            if (raycastStartSpot == null)
				raycastStartSpot = gameObject.transform;

            // Убедитесь, что положение дульных эффектов не равно null
            if (muzzleEffectsPosition == null)
				muzzleEffectsPosition = gameObject.transform;

            // Убедитесь, что место появления снаряда не находится null
            if (projectileSpawnSpot == null)
				projectileSpawnSpot = gameObject.transform;

            // Убедитесь, что модель оружия не null
            if (weaponModel == null)
				weaponModel = gameObject;

            // Убедитесь, что Перекрестие прицела не является null
            if (crosshairTexture == null)
				crosshairTexture = new Texture2D(0, 0);

            // Инициализируйте список пулов пулевых отверстий
            for (int i = 0; i < bulletHolePoolNames.Count; i++)
			{
				GameObject g = GameObject.Find(bulletHolePoolNames[i]);

				if (g != null && g.GetComponent<BulletHolePool>() != null)
					bulletHoleGroups[i].bulletHole = g.GetComponent<BulletHolePool>();
				else
					Debug.LogWarning("Bullet Hole Pool does not exist or does not have a BulletHolePool component.  Please assign GameObjects in the inspector that have the BulletHolePool component.");
			}

            // Инициализируйте список пулов пулевых отверстий по умолчанию
            for (int i = 0; i < defaultBulletHolePoolNames.Count; i++)
			{
				GameObject g = GameObject.Find(defaultBulletHolePoolNames[i]);

				if (g.GetComponent<BulletHolePool>() != null)
					defaultBulletHoles[i] = g.GetComponent<BulletHolePool>();
				else
					Debug.LogWarning("Default Bullet Hole Pool does not have a BulletHolePool component.  Please assign GameObjects in the inspector that have the BulletHolePool component.");
			}
		}

        // Обновление вызывается один раз за кадр
        void Update()
		{

            // Рассчитайте текущую точность для этого оружия
            currentAccuracy = Mathf.Lerp(currentAccuracy, accuracy, accuracyRecoverRate * Time.deltaTime);

            // Рассчитайте текущий размер перекрестия. Это то, что заставляет перекрестие динамически увеличиваться и уменьшаться во время съемки
            currentCrosshairSize = startingCrosshairSize + (accuracy - currentAccuracy) * 0.8f;

            // Обновите таймер включения
            fireTimer += Time.deltaTime;

            // Проверка пользовательского ввода() обрабатывает запуск на основе пользовательского ввода
            if (playerWeapon)
			{
				CheckForUserInput();
			}

            // Перезарядите оружие, если в нем закончились патроны
            if (reloadAutomatically && currentAmmo <= 0)
				Reload();

            // Восстановление отдачи
            if (playerWeapon && recoil && type != WeaponType.Beam)
			{
				weaponModel.transform.position = Vector3.Lerp(weaponModel.transform.position, transform.position, recoilRecoveryRate * Time.deltaTime);
				weaponModel.transform.rotation = Quaternion.Lerp(weaponModel.transform.rotation, transform.rotation, recoilRecoveryRate * Time.deltaTime);
			}

            // Убедитесь, что функция Stop Beam() вызывается, когда оружие больше не стреляет лучом (вызов метода Beam())
            if (type == WeaponType.Beam)
			{
				if (!beaming)
					StopBeam();
				beaming = false;    // Переменной beaming присваивается значение true для каждого кадра, в котором вызывается метод Beam()
            }
		}

        // Проверяет вводимые пользователем данные для использования оружия - только в том случае, если это оружие управляется игроком
        void CheckForUserInput()
		{

            // Стреляйте, если это оружие типа raycast и пользователь нажимает кнопку "Огонь"
            if (type == WeaponType.Raycast)
			{
				if (fireTimer >= actualROF && burstCounter < burstRate && canFire)
				{
					if (Input.GetButton("Fire1"))
					{
						if (!warmup)    // Обычное срабатывание, когда пользователь удерживает нажатой кнопку "Огонь"
                        {
							Fire();
						}
						else if (heat < maxWarmup)  // В противном случае просто продолжайте разминку до тех пор, пока пользователь не отпустит кнопку
                        {
							heat += Time.deltaTime;
						}
					}
					if (warmup && Input.GetButtonUp("Fire1"))
					{
						if (allowCancel && Input.GetButton("Cancel"))
						{
							heat = 0.0f;
						}
						else
						{
							Fire();
						}
					}
				}
			}
            // Запустите снаряд, если это оружие метательного типа, и пользователь нажмет кнопку "Огонь".
            if (type == WeaponType.Projectile)
			{
				if (fireTimer >= actualROF && burstCounter < burstRate && canFire)
				{
					if (Input.GetButton("Fire1"))
					{
						if (!warmup)    // Обычное срабатывание, когда пользователь удерживает нажатой кнопку "Огонь"
                        {
							Launch();
						}
						else if (heat < maxWarmup)  // В противном случае просто продолжайте разминку до тех пор, пока пользователь не отпустит кнопку
                        {
							heat += Time.deltaTime;
						}
					}
					if (warmup && Input.GetButtonUp("Fire1"))
					{
						if (allowCancel && Input.GetButton("Cancel"))
						{
							heat = 0.0f;
						}
						else
						{
							Launch();
						}
					}
				}

			}
            // Сброс пакета
            if (burstCounter >= burstRate)
			{
				burstTimer += Time.deltaTime;
				if (burstTimer >= burstPause)
				{
					burstCounter = 0;
					burstTimer = 0.0f;
				}
			}
            // Стреляйте лучом, если это оружие лучевого типа, и пользователь нажимает кнопку "Огонь"
            if (type == WeaponType.Beam)
			{
				if (Input.GetButton("Fire1") && beamHeat <= maxBeamHeat && !coolingDown)
				{
					Beam();
				}
				else
				{
                    // Прекрати пусукать луч
                    StopBeam();
				}
				if (beamHeat >= maxBeamHeat)
				{
					coolingDown = true;
				}
				else if (beamHeat <= maxBeamHeat - (maxBeamHeat / 2))
				{
					coolingDown = false;
				}
			}

            // Перезарядите, если нажата кнопка "Перезарядить"
            if (Input.GetButtonDown("Reload"))
				Reload();

            // Если оружие находится в полуавтоматическом режиме и пользователь отпускает кнопку, seat может стрелять в режиме true
            if (Input.GetButtonUp("Fire1"))
				canFire = true;
		}

        // Общедоступный метод, который заставляет оружие стрелять - может быть вызван из других скриптов - пока вызывайте указанную стрельбу
        public void RemoteFire()
		{
			AIFiring();
		}

        // Определяет, когда искусственный интеллект может начать стрельбу
        public void AIFiring()
		{

            // Стреляйте, если это оружие лучевого типа
            if (type == WeaponType.Raycast)
			{
				if (fireTimer >= actualROF && burstCounter < burstRate && canFire)
				{
					StartCoroutine(DelayFire());    // Срабатывает по истечении времени, указанного в разделе задержка перед срабатыванием
                }
			}
            // Запустите снаряд, если это оружие метательного типа
            if (type == WeaponType.Projectile)
			{
				if (fireTimer >= actualROF && canFire)
				{
					StartCoroutine(DelayLaunch());
				}
			}
            // Сброс пакета
            if (burstCounter >= burstRate)
			{
				burstTimer += Time.deltaTime;
				if (burstTimer >= burstPause)
				{
					burstCounter = 0;
					burstTimer = 0.0f;
				}
			}
            // Стреляйте лучом, если это оружие лучевого типа
            if (type == WeaponType.Beam)
			{
				if (beamHeat <= maxBeamHeat && !coolingDown)
				{
					Beam();
				}
				else
				{
					// Стоп пуска луча
					StopBeam();
				}
				if (beamHeat >= maxBeamHeat)
				{
					coolingDown = true;
				}
				else if (beamHeat <= maxBeamHeat - (maxBeamHeat / 2))
				{
					coolingDown = false;
				}
			}
		}

		IEnumerator DelayFire()
		{
            // Сбросьте таймер включения на 0 (для ROF)
            fireTimer = 0.0f;

            // Увеличьте счетчик пакетов
            burstCounter++;

            // Если это полуавтоматическое оружие, установите can Fire в значение false (это означает, что оружие не сможет выстрелить снова, пока игрок не отпустит кнопку "Огонь").
            if (auto == Auto.Semi)
				canFire = false;

            // Отправьте сообщение, чтобы пользователи могли выполнять другие действия всякий раз, когда это происходит
            SendMessageUpwards("OnEasyWeaponsFire", SendMessageOptions.DontRequireReceiver);

			yield return new WaitForSeconds(delayBeforeFire);
			Fire();
		}
		IEnumerator DelayLaunch()
		{
            // Сбросьте таймер включения на 0 (для ROF)
            fireTimer = 0.0f;

			// Increment the burst counter
			burstCounter++;

            // Если это полуавтоматическое оружие, установите can Fire в значение false (это означает, что оружие не сможет выстрелить снова, пока игрок не отпустит кнопку "Огонь").
            if (auto == Auto.Semi)
				canFire = false;

            // Отправьте сообщение, чтобы пользователи могли выполнять другие действия всякий раз, когда это происходит
            SendMessageUpwards("OnEasyWeaponsLaunch", SendMessageOptions.DontRequireReceiver);

			yield return new WaitForSeconds(delayBeforeFire);
			Launch();
		}
		IEnumerator DelayBeam()
		{
			yield return new WaitForSeconds(delayBeforeFire);
			Beam();
		}

        void OnGUI()
		{

            // Перекрестие прицела
            if (type == WeaponType.Projectile || type == WeaponType.Beam)
			{
				currentAccuracy = accuracy;
			}
			if (showCrosshair)
			{
                // Удерживайте местоположение центра экрана в переменной
                Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

                // Нарисуйте перекрестие прицела, основываясь на неточности оружия
                // Слева
                Rect leftRect = new Rect(center.x - crosshairLength - currentCrosshairSize, center.y - (crosshairWidth / 2), crosshairLength, crosshairWidth);
				GUI.DrawTexture(leftRect, crosshairTexture, ScaleMode.StretchToFill);
				// Справа
				Rect rightRect = new Rect(center.x + currentCrosshairSize, center.y - (crosshairWidth / 2), crosshairLength, crosshairWidth);
				GUI.DrawTexture(rightRect, crosshairTexture, ScaleMode.StretchToFill);
				// Сверху
				Rect topRect = new Rect(center.x - (crosshairWidth / 2), center.y - crosshairLength - currentCrosshairSize, crosshairWidth, crosshairLength);
				GUI.DrawTexture(topRect, crosshairTexture, ScaleMode.StretchToFill);
				// Снизу
				Rect bottomRect = new Rect(center.x - (crosshairWidth / 2), center.y + currentCrosshairSize, crosshairWidth, crosshairLength);
				GUI.DrawTexture(bottomRect, crosshairTexture, ScaleMode.StretchToFill);
			}

            // Дисплей с боеприпасами
            if (showCurrentAmmo)
			{
				if (type == WeaponType.Raycast || type == WeaponType.Projectile)
					GUI.Label(new Rect(10, Screen.height - 30, 100, 20), "Ammo: " + currentAmmo);
				else if (type == WeaponType.Beam)
					GUI.Label(new Rect(10, Screen.height - 30, 100, 20), "Heat: " + (int)(beamHeat * 100) + "/" + (int)(maxBeamHeat * 100));
			}
		}


        // Система лучевого литья
        void Fire()
		{
            // Сбросьте таймер включения на 0 (для ROF)
            fireTimer = 0.0f;

            // Увеличьте счетчик пакетов
            burstCounter++;

            // Если это полуавтоматическое оружие, установите can Fire в значение false (это означает, что оружие не сможет выстрелить снова, пока игрок не отпустит кнопку "Огонь").
            if (auto == Auto.Semi)
				canFire = false;

            // Сначала убедитесь, что есть боеприпасы
            if (currentAmmo <= 0)
			{
				DryFire();
				return;
			}

            // Вычтите 1 из текущего количества боеприпасов
            if (!infiniteAmmo)
				currentAmmo--;


            // Стреляйте один раз за каждый выстрел в расчете на один раунд.
            for (int i = 0; i < shotPerRound; i++)
			{
                // Рассчитайте точность для этого выстрела
                float accuracyVary = (100 - currentAccuracy) / 1000;
				Vector3 direction = raycastStartSpot.forward;
				direction.x += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
				direction.y += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
				direction.z += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
				currentAccuracy -= accuracyDropPerShot;
				if (currentAccuracy <= 0.0f)
					currentAccuracy = 0.0f;

                // Луч, который будет использован для этого снимка
                Ray ray = new Ray(raycastStartSpot.position, direction);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit, range))
				{
                    // Перегрев зарядки
                    float damage = power;
					if (warmup)
					{
						damage *= heat * powerMultiplier;
						heat = 0.0f;
					}

					// Урон
					hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -damage, SendMessageOptions.DontRequireReceiver);

					if (shooterAIEnabled)
					{
						hit.transform.SendMessageUpwards("Damage", damage / 100, SendMessageOptions.DontRequireReceiver);
					}
                    if (bloodyMessEnabled)
					{
                        //вызовите функцию Apply Damage() в скрипте настройки вражеского персонажа

                        if (hit.collider.gameObject.tag == "Player")
						{
							Vector3 directionShot = hit.collider.transform.position - transform.position;

                            //  Раскомментируйте следующий раздел для совместимости с Bloody Mess
                            /*
							if (hit.collider.gameObject.GetComponent<Limb>())
							{
								GameObject parent = hit.collider.gameObject.GetComponent<Limb>().parent;
								CharacterSetup character = parent.GetComponent<CharacterSetup>();
								character.ApplyDamage(damage, hit.collider.gameObject, weaponType, directionShot, Camera.main.transform.position);
							}
							*/
                        }
                    }

                    // Пулевые отверстия

                    // Убедитесь, что объект hit GameObject не определен как исключение для пулевых отверстий
                    bool exception = false;
					if (bhSystem == BulletHoleSystem.Tag)
					{
						foreach (SmartBulletHoleGroup bhg in bulletHoleExceptions)
						{
							if (hit.collider.gameObject.tag == bhg.tag)
							{
								exception = true;
								break;
							}
						}
					}
					else if (bhSystem == BulletHoleSystem.Material)
					{
						foreach (SmartBulletHoleGroup bhg in bulletHoleExceptions)
						{
							MeshRenderer mesh = FindMeshRenderer(hit.collider.gameObject);
							if (mesh != null)
							{
								if (mesh.sharedMaterial == bhg.material)
								{
									exception = true;
									break;
								}
							}
						}
					}
					else if (bhSystem == BulletHoleSystem.Physic_Material)
					{
						foreach (SmartBulletHoleGroup bhg in bulletHoleExceptions)
						{
							if (hit.collider.sharedMaterial == bhg.physicMaterial)
							{
								exception = true;
								break;
							}
						}
					}

                    // Выберите пулы пулевых отверстий, если нет исключений
                    if (makeBulletHoles && !exception)
					{
                        // Список готовых конструкций с пулевыми отверстиями на выбор
                        List<SmartBulletHoleGroup> holes = new List<SmartBulletHoleGroup>();

                        // Отобразите группы пулевых отверстий на основе тегов
                        if (bhSystem == BulletHoleSystem.Tag)
						{
							foreach (SmartBulletHoleGroup bhg in bulletHoleGroups)
							{
								if (hit.collider.gameObject.tag == bhg.tag)
								{
									holes.Add(bhg);
								}
							}
						}

                        // Отображение групп пулевых отверстий в зависимости от материалов
                        else if (bhSystem == BulletHoleSystem.Material)
						{
                            // Получите сетку, по которой был нанесен удар, если таковая имеется
                            MeshRenderer mesh = FindMeshRenderer(hit.collider.gameObject);

							foreach (SmartBulletHoleGroup bhg in bulletHoleGroups)
							{
								if (mesh != null)
								{
									if (mesh.sharedMaterial == bhg.material)
									{
										holes.Add(bhg);
									}
								}
							}
						}

                        // Отображение групп пулевых отверстий на основе физических материалов
                        else if (bhSystem == BulletHoleSystem.Physic_Material)
						{
							foreach (SmartBulletHoleGroup bhg in bulletHoleGroups)
							{
								if (hit.collider.sharedMaterial == bhg.physicMaterial)
								{
									holes.Add(bhg);
								}
							}
						}


						SmartBulletHoleGroup sbhg = null;

                        // Если для этого параметра не были указаны отверстия для пуль, используйте отверстия для пуль по умолчанию
                        if (holes.Count == 0)   // Если не было обнаружено пригодных для использования (для этого игрового объекта) пулевых отверстий...
                        {
							List<SmartBulletHoleGroup> defaultsToUse = new List<SmartBulletHoleGroup>();
							foreach (BulletHolePool h in defaultBulletHoles)
							{
								defaultsToUse.Add(new SmartBulletHoleGroup("Default", null, null, h));
							}

                            // Выберите пулевое отверстие наугад из списка
                            sbhg = defaultsToUse[Random.Range(0, defaultsToUse.Count)];
						}

                        // Создайте реальный игровой объект с пулевым отверстием
                        else
                        {
                            // Выберите пулевое отверстие наугад из списка
                            sbhg = holes[Random.Range(0, holes.Count)];
						}

                        // Поместите пулевое отверстие на место происшествия
                        if (sbhg.bulletHole != null)
							sbhg.bulletHole.PlaceBulletHole(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
					}

                    // Эффекты попадания
                    if (makeHitEffects)
					{
						foreach (GameObject hitEffect in hitEffects)
						{
							if (hitEffect != null)
								Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
						}
					}

                    // Добавьте силу к объекту, по которому был нанесен удар
                    if (hit.rigidbody)
					{
						hit.rigidbody.AddForce(ray.direction * power * forceMultiplier);
					}
				}
			}

			// Отдача
			if (recoil)
				Recoil();

            // Эффекты дульной вспышки
            if (makeMuzzleEffects)
			{
				GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
				if (muzfx != null)
					Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation);
			}

            // Создание экземпляра реквизита оболочки
            if (spitShells)
			{
				GameObject shellGO = Instantiate(shell, shellSpitPosition.position, shellSpitPosition.rotation) as GameObject;
				shellGO.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(shellSpitForce + Random.Range(0, shellForceRandom), 0, 0), ForceMode.Impulse);
				shellGO.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(shellSpitTorqueX + Random.Range(-shellTorqueRandom, shellTorqueRandom), shellSpitTorqueY + Random.Range(-shellTorqueRandom, shellTorqueRandom), 0), ForceMode.Impulse);
			}

            // Воспроизведите звук выстрела
            GetComponent<AudioSource>().PlayOneShot(fireSound);
		}

        [Command]
        private void cmdFire()
        {
            
        }

        // Система метания снарядов
        public void Launch()
		{
            // Сбросьте таймер включения на 0 (для ROF)
            fireTimer = 0.0f;

            // Увеличьте счетчик пакетов
            burstCounter++;

            // Если это полуавтоматическое оружие, установите can Fire в значение false (это означает, что оружие не сможет выстрелить снова, пока игрок не отпустит кнопку "Огонь").
            if (auto == Auto.Semi)
				canFire = false;

            // Сначала убедитесь, что есть боеприпасы
            if (currentAmmo <= 0)
			{
				DryFire();
				return;
			}

            // Вычтите 1 из текущего количества боеприпасов
            if (!infiniteAmmo)
				currentAmmo--;

            // Стреляйте один раз за каждый выстрел в расчете на один раунд.
            for (int i = 0; i < shotPerRound; i++)
			{
                // Создайте экземпляр снаряда
                if (projectile != null)
				{
					GameObject proj = Instantiate(projectile, projectileSpawnSpot.position, projectileSpawnSpot.rotation) as GameObject;

					// Перегрев зарядки
					if (warmup)
					{
						if (multiplyPower)
							proj.SendMessage("MultiplyDamage", heat * powerMultiplier, SendMessageOptions.DontRequireReceiver);
						if (multiplyForce)
							proj.SendMessage("MultiplyInitialForce", heat * initialForceMultiplier, SendMessageOptions.DontRequireReceiver);

						heat = 0.0f;
					}
				}
				else
				{
					Debug.Log("Projectile to be instantiated is null.  Make sure to set the Projectile field in the inspector.");
				}
			}

			// Отдача
			if (recoil)
				Recoil();

			// Эффекты дульной вспышки
			if (makeMuzzleEffects)
			{
				GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
				if (muzfx != null)
					Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation);
			}

            // Создание экземпляра реквизита оболочки
            if (spitShells)
			{
				GameObject shellGO = Instantiate(shell, shellSpitPosition.position, shellSpitPosition.rotation) as GameObject;
				shellGO.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(shellSpitForce + Random.Range(0, shellForceRandom), 0, 0), ForceMode.Impulse);
				shellGO.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(shellSpitTorqueX + Random.Range(-shellTorqueRandom, shellTorqueRandom), shellSpitTorqueY + Random.Range(-shellTorqueRandom, shellTorqueRandom), 0), ForceMode.Impulse);
			}

            // Воспроизведите звук выстрела
            GetComponent<AudioSource>().PlayOneShot(fireSound);
		}

        // Лучевая система
        void Beam()
		{
            // Отправьте сообщение, чтобы пользователи могли выполнять другие действия всякий раз, когда это происходит
            SendMessageUpwards("OnEasyWeaponsBeaming", SendMessageOptions.DontRequireReceiver);

            // Установите для переменной beaming значение true
            beaming = true;

            // Заставьте лучевое оружие нагреваться по мере его использования
            if (!infiniteBeam)
				beamHeat += Time.deltaTime;

            // Создайте эффект луча, если он еще не был создан. Эта система использует средство визуализации линий для пустого экземпляра GameObject
            if (beamGO == null)
			{
				beamGO = new GameObject(beamTypeName, typeof(LineRenderer));
				beamGO.transform.parent = transform;        // Сделайте объект beam дочерним по отношению к объекту weapon, чтобы при деактивации оружия луч также был beamGO.transform.SetParent(преобразование), который работает только в Unity 4.6 или новее;
            }
			LineRenderer beamLR = beamGO.GetComponent<LineRenderer>();
			beamLR.material = beamMaterial;
			beamLR.material.SetColor("_TintColor", beamColor);
			beamLR.startWidth = startBeamWidth;
			beamLR.endWidth = endBeamWidth;

            // Количество отражений
            int reflections = 0;

            // Все точки, в которых отражается лазерный луч
            List<Vector3> reflectionPoints = new List<Vector3>();
			// Add the first point to the list of beam reflection points
			reflectionPoints.Add(raycastStartSpot.position);

            // Удерживайте переменную для последней отраженной точки
            Vector3 lastPoint = raycastStartSpot.position;

            // Объявляйте переменные для вычисления лучей
            Vector3 incomingDirection;
			Vector3 reflectDirection;

            // Независимо от того, нужно ли лучу продолжать отражаться или нет
            bool keepReflecting = true;

            // Отбрасывание лучей (повреждение и т.д.)
            Ray ray = new Ray(lastPoint, raycastStartSpot.forward);
			RaycastHit hit;

			do
			{
                // Инициализируйте следующую точку. Если попадание в raycast не возвращается, это будет прямое направление * диапазон
                Vector3 nextPoint = ray.direction * range;

				if (Physics.Raycast(ray, out hit, range))
				{
                    // Установите следующую точку на место попадания из выпущенного луча
                    nextPoint = hit.point;

                    // Рассчитайте следующее направление, в котором будет направлен луч
                    incomingDirection = nextPoint - lastPoint;
					reflectDirection = Vector3.Reflect(incomingDirection, hit.normal);
					ray = new Ray(nextPoint, reflectDirection);

                    // Обновите последнюю переменную точки
                    lastPoint = hit.point;

                    // Эффекты попадания
                    if (makeHitEffects)
					{
						foreach (GameObject hitEffect in hitEffects)
						{
							if (hitEffect != null)
								Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
						}
					}

					// Урон
					hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -beamPower, SendMessageOptions.DontRequireReceiver);

                    // Поддержка искусственного интеллекта шутера
                    if (shooterAIEnabled)
					{
						hit.transform.SendMessageUpwards("Damage", beamPower / 100, SendMessageOptions.DontRequireReceiver);
					}

                    // Поддержка кровавого месива
                    if (bloodyMessEnabled)
					{
                        //вызовите функцию Apply Damage() в скрипте настройки вражеского персонажа
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Limb"))
						{
							Vector3 directionShot = hit.collider.transform.position - transform.position;

                            //  Удалите метки комментариев из следующего раздела кода для поддержки Bloody Mess
                            /*
							if (hit.collider.gameObject.GetComponent<Limb>())
							{
								GameObject parent = hit.collider.gameObject.GetComponent<Limb>().parent;

								CharacterSetup character = parent.GetComponent<CharacterSetup>();
								character.ApplyDamage(beamPower, hit.collider.gameObject, weaponType, directionShot, Camera.main.transform.position);
							}
							*/

                        }
                    }


                    // Увеличьте счетчик отражений
                    reflections++;
				}
				else
				{

					keepReflecting = false;
				}

                // Добавьте следующую точку в список точек отражения луча
                reflectionPoints.Add(nextPoint);

			} while (keepReflecting && reflections < maxReflections && reflect && (reflectionMaterial == null || (FindMeshRenderer(hit.collider.gameObject) != null && FindMeshRenderer(hit.collider.gameObject).sharedMaterial == reflectionMaterial)));

            // Установите положение вершин луча средства визуализации линий
            beamLR.positionCount = reflectionPoints.Count;
			for (int i = 0; i < reflectionPoints.Count; i++)
			{
				beamLR.SetPosition(i, reflectionPoints[i]);

                // Эффекты отражения дула
                if (makeMuzzleEffects && i > 0)     // Не создает FX на первой итерации, поскольку это обрабатывается позже. Это делается для того, чтобы FX в точке дула мог быть приученный к оружию
                {
					GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
					if (muzfx != null)
					{
						Instantiate(muzfx, reflectionPoints[i], muzzleEffectsPosition.rotation);
					}
				}
			}

            // Эффекты дульной вспышки
            if (makeMuzzleEffects)
			{
				GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
				if (muzfx != null)
				{
					GameObject mfxGO = Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation) as GameObject;
					mfxGO.transform.parent = raycastStartSpot;
				}
			}

            // Воспроизведите звук лучевого огня
            if (!GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().clip = fireSound;
				GetComponent<AudioSource>().Play();
			}
		}
        public void StopBeam()
		{
            // Перезапустите таймер луча
            beamHeat -= Time.deltaTime;
			if (beamHeat < 0)
				beamHeat = 0;
			GetComponent<AudioSource>().Stop();

            // Удалите игровой объект с эффектом видимого луча
            if (beamGO != null)
			{
				Destroy(beamGO);
			}

            //Отправьте сообщение, чтобы пользователи могли выполнять другие действия всякий раз, когда это происходит
            SendMessageUpwards("OnEasyWeaponsStopBeaming", SendMessageOptions.DontRequireReceiver);
		}


        // Перезарядите оружие
        void Reload()
		{
			currentAmmo = ammoCapacity;
			fireTimer = -reloadTime;
			GetComponent<AudioSource>().PlayOneShot(reloadSound);

            // Отправьте сообщение, чтобы пользователи могли выполнять другие действия всякий раз, когда это происходит
            SendMessageUpwards("OnEasyWeaponsReload", SendMessageOptions.DontRequireReceiver);
		}

        // Когда оружие пытается выстрелить без каких-либо боеприпасов

        void DryFire()
		{
			GetComponent<AudioSource>().PlayOneShot(dryFireSound);
		}


        // Эффект отдачи. Это "удар", который вы видите, когда оружие движется назад во время стрельбы
        void Recoil()
		{
            // Отсутствие отдачи для ИИ
            if (!playerWeapon)
				return;

            // Убедитесь, что пользователь не оставил поле модель оружия пустым
            if (weaponModel == null)
			{
				Debug.Log("Weapon Model is null.  Make sure to set the Weapon Model field in the inspector.");
				return;
			}

            // Вычислите случайные значения для положения отдачи и поворота
            float kickBack = Random.Range(recoilKickBackMin, recoilKickBackMax);
			float kickRot = Random.Range(recoilRotationMin, recoilRotationMax);

            // Примените случайные значения к положению и повороту оружия
            weaponModel.transform.Translate(new Vector3(0, 0, -kickBack), Space.Self);
			weaponModel.transform.Rotate(new Vector3(-kickRot, 0, 0), Space.Self);
		}

        // Найдите средство визуализации сетки в указанном игровом объекте, его дочерних элементах или его родителях
        MeshRenderer FindMeshRenderer(GameObject go)
		{
			MeshRenderer hitMesh;

            // Используйте средство визуализации сетки непосредственно из этого игрового объекта, если оно у него есть
            if (go.GetComponent<Renderer>() != null)
			{
				hitMesh = go.GetComponent<MeshRenderer>();
			}

            // Попробуйте найти дочерний или родительский GameObject, у которого есть MeshRenderer
            else
            {
                // Найдите средство визуализации в дочерних игровых объектах
                hitMesh = go.GetComponentInChildren<MeshRenderer>();

                // Если средство визуализации по-прежнему не найдено, попробуйте родительские GameObjects
                if (hitMesh == null)
				{
					GameObject curGO = go;
					while (hitMesh == null && curGO.transform != curGO.transform.root)
					{
						curGO = curGO.transform.parent.gameObject;
						hitMesh = curGO.GetComponent<MeshRenderer>();
					}
				}
			}

			return hitMesh;
		}
	}
}


