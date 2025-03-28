# 苏州大学综合项目实践作业 - our_game

## 1. 需要创建的GameObject

### (1) 场景主要对象
| GameObject名称       | 作用                 | 依赖组件                              |
|----------------------|----------------------|-------------------------------------|
| BattleScene（根对象） | 组织战斗场景         | -                                   |
| MainCamera           | 摄像机视角控制       | Camera, CinemachineBrain            |
| Background           | 战斗背景             | SpriteRenderer                      |
| Ground               | 地面碰撞             | BoxCollider2D                       |
| Player               | 主角                 | Rigidbody2D, Animator, BoxCollider2D|
| Boss                 | BOSS角色             | Rigidbody2D, Animator, BoxCollider2D|
| UI                   | 血条、技能CD等UI元素 | Canvas                              |
| BulletContainer      | 存放所有子弹         | -                                   |

### (2) 子对象
- **Player 子对象**：
  - AttackPoint（空物体，近战攻击检测点）
- **Boss 子对象**：
  - FirePoint（子弹发射位置）

---

## 2. 需要创建的Scripts

| 脚本文件              | 作用                      | 绑定的对象       |
|-----------------------|--------------------------|------------------|
| PlayerController.cs   | 控制玩家移动、跳跃、攻击 | Player          |
| PlayerHealth.cs       | 处理玩家血量系统         | Player          |
| Boss.cs               | 处理BOSS行为、血量、攻击 | Boss            |
| BossAttack.cs         | 控制BOSS的攻击逻辑       | Boss            |
| Bullet.cs             | 处理子弹运动             | BulletPrefab    |
| HomingBullet.cs       | 处理追踪子弹             | BulletPrefab变体|
| WaveBullet.cs         | 处理波浪弹幕             | BulletPrefab变体|
| GameManager.cs        | 处理战斗逻辑（胜利/失败）| BattleScene     |
| HealthBar.cs          | 更新UI血条               | UI              |
| CameraController.cs   | 摄像机跟随               | MainCamera      |

---

## 3. 需要创建的Prefabs

| 预制体名称            | 作用               | 绑定的脚本                  |
|-----------------------|-------------------|----------------------------|
| PlayerPrefab          | 预设玩家角色       | PlayerController, PlayerHealth|
| BossPrefab            | 预设BOSS角色       | Boss, BossAttack           |
| BulletPrefab          | 直线子弹           | Bullet                     |
| HomingBulletPrefab    | 追踪子弹           | HomingBullet               |
| WaveBulletPrefab      | 波浪弹幕           | WaveBullet                 |
| HealthBarPrefab       | UI血条             | HealthBar                  |
| ExplosionEffectPrefab | 爆炸特效           | -                          |

