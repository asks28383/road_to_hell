# our_game
苏州大学综合项目实践作业
1.需要创建的GameObject
(1) 场景主要对象
GameObject 名称	作用	依赖组件
BattleScene（根对象）	组织战斗场景	-
MainCamera	负责摄像机视角	Camera，CinemachineBrain
Background	战斗背景	SpriteRenderer
Ground	地面碰撞	BoxCollider2D
Player	主角	Rigidbody2D，Animator，BoxCollider2D
Boss	BOSS角色	Rigidbody2D，Animator，BoxCollider2D
UI	血条、技能CD等	Canvas
BulletContainer	存放战斗中所有子弹	-
(2) 子对象
●Player 下的子对象： 
●AttackPoint（空物体，作为近战攻击检测点）
●Boss 下的子对象： 
●FirePoint（子弹发射位置）

2.需要创建的Scripts
脚本文件	作用	绑定的对象
PlayerController.cs	控制玩家移动、跳跃、攻击	Player
PlayerHealth.cs	处理玩家血量系统	Player
Boss.cs	处理BOSS行为、血量、攻击	Boss
BossAttack.cs	控制BOSS的攻击逻辑	Boss
Bullet.cs	处理子弹的运动	BulletPrefab
HomingBullet.cs	处理追踪子弹	BulletPrefab（变体）
WaveBullet.cs	处理波浪弹幕	BulletPrefab（变体）
GameManager.cs	处理战斗逻辑，比如游戏胜利/失败	BattleScene
HealthBar.cs	更新UI中的血条	UI
CameraController.cs	处理摄像机跟随	MainCamera

3.需要创建的Prefabs
预制体名称	作用	绑定的脚本
PlayerPrefab	预设玩家角色	PlayerController，PlayerHealth
BossPrefab	预设BOSS角色	Boss，BossAttack
BulletPrefab	直线子弹	Bullet
HomingBulletPrefab	追踪子弹	HomingBullet
WaveBulletPrefab	波浪弹幕	WaveBullet
HealthBarPrefab	UI血条	HealthBar
ExplosionEffectPrefab	爆炸特效	-

4.目录结构
/Assets
│── /Scenes
│   ├── BattleScene.unity
│
│── /Prefabs
│   ├── PlayerPrefab.prefab
│   ├── BossPrefab.prefab
│   ├── BulletPrefab.prefab
│   ├── HomingBulletPrefab.prefab
│   ├── WaveBulletPrefab.prefab
│   ├── HealthBarPrefab.prefab
│   ├── ExplosionEffectPrefab.prefab
│
│── /Scripts
│   ├── Player
│   │   ├── PlayerController.cs
│   │   ├── PlayerHealth.cs
│   │
│   ├── Boss
│   │   ├── Boss.cs
│   │   ├── BossAttack.cs
│   │
│   ├── Bullet
│   │   ├── Bullet.cs
│   │   ├── HomingBullet.cs
│   │   ├── WaveBullet.cs
│   │
│   ├── UI
│   │   ├── HealthBar.cs
│   │
│   ├── GameManager.cs
│   ├── CameraController.cs
│
│── /Art
│   ├── Backgrounds
│   ├── Player
│   ├── Boss
│   ├── Effects
│
│── /Animations
│   ├── Player.anim
│   ├── Boss.anim
│   ├── Effects.anim

5.物体之间的关系
(1) 主要对象的层级关系
BattleScene
│── MainCamera
│── Background
│── Ground
│── UI
│   ├── HealthBar (Boss)
│   ├── HealthBar (Player)
│── BulletContainer
│── Player
│   ├── AttackPoint
│── Boss
│   ├── FirePoint
(2) 组件之间的依赖关系
●PlayerController.cs 依赖 PlayerHealth.cs
●Boss.cs 依赖 BossAttack.cs
●BossAttack.cs 依赖 Bullet.cs
●Bullet.cs 依赖 GameManager.cs（用于通知游戏状态）
●HealthBar.cs 依赖 PlayerHealth.cs 和 Boss.cs
