/* SandStrike — офлайн FPS. Вся графика процедурная, Three.js из локального ассета. */
(function () {
'use strict';

var W = function (name, proto, cls, dmg, rpm, mag, spread, recoil, range, arch, body, metal, scale) {
  return {name:name, proto:proto, cls:cls, dmg:dmg, rpm:rpm, mag:mag, spread:spread,
    recoil:recoil, range:range, arch:arch, body:body, metal:metal, scale:scale,
    move:cls===5?1.12:cls===3?.78:cls===2?.95:cls===1?1.03:1};
};
var WEAPONS = [
  W('Кремень','USP-S',0,34,285,12,4,7,14,0,0x3b4148,0xa9afb6,.95),
  W('Гром','P250',0,31,333,13,6,9,13,0,0x4a4f52,0xbfc5c8,.95),
  W('Песчанка','Desert Eagle',0,58,143,7,9,22,18,0,0x6b6255,0xd8d2c6,1.12),
  W('Искра','Glock-18',0,28,375,18,7,8,12,0,0x2f3539,0x9da5a9,.90),
  W('Шмель','Five-SeveN',0,29,400,20,5,7,13,0,0x4c5147,0xb9beaf,.92),
  W('Ятаган','Tec-9',0,33,375,18,9,11,12,0,0x55504a,0xc9bfb2,1),
  W('Двойка','Dual Berettas',0,26,500,30,11,10,11,0,0x565b5e,0xc6ccce,.86),
  W('Метла','MP7',1,24,667,30,7,6,15,0,0x343a3f,0xaeb4b8,1.10),
  W('Оса','MP9',1,23,750,30,9,6,14,0,0x3d4245,0xb6bcc0,1.05),
  W('Глыба','UMP-45',1,31,545,25,8,9,16,1,0x414744,0xbec4bc,1.05),
  W('Рой','P90',1,22,833,50,10,5,15,2,0x42484e,0xb4bbc0,1.15),
  W('Кусок','MAC-10',1,26,800,30,11,8,12,0,0x4a4e4a,0xc2c7c1,1),
  W('Стрела','MP5',1,27,632,30,7,7,16,1,0x373c40,0xaeb5b9,1.05),
  W('Буря','AK-47',2,36,571,30,7,15,22,1,0x6a4b32,0xc8b79b,1.20),
  W('Клин','M4A1',2,33,632,30,6,12,23,1,0x3a4147,0xaeb6bb,1.18),
  W('Грань','M4A1-S',2,35,571,25,5,12,24,1,0x42484b,0xb7bdc0,1.16),
  W('Заря','AKR',2,34,600,30,6,14,22,1,0x5e4632,0xbfac90,1.18),
  W('Заря-12','AKR-12',2,38,522,30,8,17,21,1,0x57412f,0xb9a487,1.25),
  W('Вестник','M16',2,32,600,30,6,11,24,1,0x3f454a,0xb2b8bd,1.20),
  W('Скальпель','Galil',2,35,571,30,7,14,21,1,0x4d4a3e,0xb9b49b,1.15),
  W('Копьё','FAMAS',2,34,545,25,6,13,22,1,0x3f4446,0xaeb4b6,1.12),
  W('Сфера','AUG',2,33,632,30,5,11,24,2,0x414745,0xb4bab5,1.10),
  W('Глаз','AWP',3,115,41,5,3,34,40,3,0x2f4a47,0xb7c7c2,1.35),
  W('Даль','AWM',3,108,38,5,3,36,42,3,0x3c4b3a,0xbfc8b5,1.35),
  W('Точка','SSG 08',3,82,50,10,4,26,32,1,0x4a4537,0xc0b79d,1.15),
  W('Прицел','SCAR-20',3,80,109,20,5,24,30,3,0x3e4448,0xb0b7bc,1.25),
  W('Толкач','XM1014',4,22,71,6,24,26,9,1,0x5a4330,0xc1ae92,1.20),
  W('Дробь','Nova',4,26,67,8,26,28,8,1,0x50483c,0xb9b09e,1.18),
  W('Обрез','Sawed-Off',4,32,75,6,30,30,6,1,0x5c4632,0xc6b393,.95),
  W('Бабочка','Butterfly Knife',5,55,143,1,0,0,2,4,0x6e757a,0xd8dee0,1)
];
var CLASS_NAMES=['ПИСТОЛЕТ','ПП','АВТОМАТ','СНАЙПЕРКА','ДРОБОВИК','НОЖ'];
var SK = function(name,rarity,a,b,c){return {name:name,rarity:rarity,a:a,b:b,c:c};};
var CSKINS=[
  SK('Песчаный дозор',0,0xc2a878,0x6e6047,0xe2b866),SK('Графит',0,0x4a5056,0x2c3136,0x9aa3aa),
  SK('Хаки',0,0x7c8468,0x4b523f,0xc3c9a7),SK('Кобальт',1,0x355a8c,0x1f3a60,0x7cb2e8),
  SK('Лесник',1,0x3e6148,0x263d2d,0x8fc79b),SK('Красный песок',1,0x9b4b3c,0x5e2d25,0xe58368),
  SK('Полярный',2,0xd7dee2,0x8fa2af,0xffffff),SK('Тень',2,0x2e3033,0x17191b,0x666b70),
  SK('Золото',3,0xc9a44a,0x7c6123,0xffe29a),SK('Неон',3,0x2cd3c0,0x126d68,0xa6fff4),
  SK('Кварц',2,0xd8a7ae,0x8b666d,0xffd6dc),SK('Радар',3,0xe07a32,0x7a3f1a,0xffc08a)
];
var GSKINS=[
  SK('Сталь',0,0x6e767d,0x3a4045,0xd5dbde),SK('Песок',0,0xb59b70,0x6d5c41,0xebd6a8),
  SK('Карбон',1,0x3d4246,0x202427,0x7a8288),SK('Хром',1,0xbcc6cb,0x748085,0xffffff),
  SK('Золото',2,0xceaa4f,0x806327,0xffe7a6),SK('Неон',2,0x23c9b6,0x116a62,0x9dfff1),
  SK('Коралл',2,0xc75b4b,0x74352b,0xffa38e),SK('Лёд',2,0x9fc9de,0x5b7e96,0xe8f7ff),
  SK('Яд',3,0x8db93f,0x42601c,0xd6ff92),SK('Плазма',3,0x9b5bd6,0x4c2a75,0xe0baff),
  SK('Заря',3,0xe5622f,0x7c3317,0xffbe8c),SK('Графит',1,0x4b4f54,0x272a2d,0x8c9297)
];
var RARITY=['#A7ADB4','#63B9F5','#C77DE8','#FFC857'];
var MAP_NAMES=['Песчаник / SANDSTONE','Каньон','Руины'];

var S={weapon:13,gskin:0,cskin:0,bots:4,diff:1,sens:100,sound:true,map:0,kills:0,wins:0};
try{var raw=localStorage.getItem('sandstrike');if(raw){var o=JSON.parse(raw);for(var k in S)if(o[k]!==undefined)S[k]=o[k];}}catch(e){}
S.weapon=Math.max(0,Math.min(29,S.weapon|0));S.gskin=Math.max(0,Math.min(11,S.gskin|0));
S.cskin=Math.max(0,Math.min(11,S.cskin|0));S.bots=Math.max(1,Math.min(9,S.bots|0));
S.diff=Math.max(0,Math.min(2,S.diff|0));S.sens=Math.max(40,Math.min(220,S.sens|0));
S.map=Math.max(0,Math.min(2,S.map|0));
function save(){try{localStorage.setItem('sandstrike',JSON.stringify(S));}catch(e){}}

var renderer=new THREE.WebGLRenderer({antialias:true,powerPreference:'high-performance'});
renderer.setPixelRatio(Math.min(window.devicePixelRatio||1,1.5));
renderer.setSize(innerWidth,innerHeight);
document.getElementById('gl').appendChild(renderer.domElement);
var scene=new THREE.Scene();
scene.background=new THREE.Color(0x14171b);
scene.fog=new THREE.Fog(0x6b5b45,25,80);
var camera=new THREE.PerspectiveCamera(70,innerWidth/innerHeight,.02,200);
scene.add(camera);
scene.add(new THREE.HemisphereLight(0xffffff,0x534738,1.15));
var sun=new THREE.DirectionalLight(0xfff0d0,1.3);sun.position.set(12,22,9);scene.add(sun);
var GEO=new THREE.BoxGeometry(1,1,1);
var CYL=new THREE.CylinderGeometry(1,1,1,12);

function mixColor(a,b,t){var x=new THREE.Color(a),y=new THREE.Color(b);return x.lerp(y,t).getHex();}
function mat(c,metal){return new THREE.MeshStandardMaterial({color:c,metalness:metal||0,roughness:.72});}
function box(parent,pos,scale,material){var o=new THREE.Mesh(GEO,material);o.position.set(pos[0],pos[1],pos[2]);o.scale.set(scale[0],scale[1],scale[2]);parent.add(o);return o;}
function cyl(parent,pos,scale,material,rot){var o=new THREE.Mesh(CYL,material);o.position.set(pos[0],pos[1],pos[2]);o.scale.set(scale[0],scale[1],scale[2]);if(rot)o.rotation.set(rot[0],rot[1],rot[2]);parent.add(o);return o;}

function makeWeapon(def,skin){
  var g=new THREE.Group();
  var body=mat(mixColor(def.body,skin.a,.68),.25), metal=mat(mixColor(def.metal,skin.c,.45),.8), accent=mat(skin.c,.5);
  if(def.cls===5) return makeButterfly(skin);
  var len=def.cls===3?1.35:def.cls===0?.52:.95;
  box(g,[0,0,0],[.16,.17,len*.55],body);
  cyl(g,[0,.02,len*.5],[.045,len*.5,.045],metal,[Math.PI/2,0,0]);
  box(g,[0,-.2,-.12],[.1,.32,.12],body);
  box(g,[0,-.2,.12],[.11,.34,.14],metal);
  if(def.cls!==0){box(g,[0,0,-len*.52],[.12,.14,len*.4],body);}
  box(g,[0,.13,.08],[.05,.06,.12],accent);
  if(def.cls===3){cyl(g,[0,.19,.08],[.07,.3,.07],metal,[Math.PI/2,0,0]);}
  g.scale.setScalar(def.scale);
  return g;
}
function makeButterfly(skin){
  var root=new THREE.Group();
  var metal=mat(mixColor(0xd8dee0,skin.c,.4),.85),grip=mat(skin.b,.1),accent=mat(skin.c,.5);
  var pivots=[];
  for(var s=-1;s<=1;s+=2){
    var p=new THREE.Group();p.position.x=s*.14;root.add(p);pivots.push(p);
    box(p,[s*.18,0,0],[.34,.07,.08],grip);
    box(p,[s*.12,.04,0],[.06,.02,.085],accent);
    box(p,[s*.58,0,0],[.5,.025,.09],metal);
    cyl(p,[0,0,0],[.05,.05,.05],metal,[Math.PI/2,0,0]);
  }
  root.userData.pivots=pivots;root.userData.flip=Math.random()*6;
  return root;
}
function animateButterfly(g,dt,speed){
  if(!g||!g.userData.pivots)return;
  g.userData.flip+=dt*(speed||2.2);
  var a=Math.sin(g.userData.flip)*2.6;
  g.userData.pivots[0].rotation.z=a;
  g.userData.pivots[1].rotation.z=Math.PI-a;
}
function makeCharacter(skin,team){
  var root=new THREE.Group();
  var cloth=mat(skin.a,.05),vest=mat(skin.b,.2),accent=mat(skin.c,.5),boots=mat(0x24272b,.2);
  var torso=new THREE.Group();torso.position.y=1.02;root.add(torso);
  box(torso,[0,0,0],[.66,.82,.36],cloth);
  box(torso,[0,.04,.2],[.6,.6,.1],vest);
  box(torso,[0,-.37,0],[.7,.1,.4],accent);
  box(torso,[-.43,0,0],[.16,.75,.18],cloth);
  box(torso,[.43,0,0],[.16,.75,.18],cloth);
  var head=new THREE.Group();head.position.y=1.62;root.add(head);
  var skull=new THREE.Mesh(new THREE.SphereGeometry(.19,12,10),cloth);head.add(skull);
  box(head,[0,.02,.17],[.3,.1,.07],accent);
  box(head,[0,.17,0],[.38,.12,.38],vest);
  box(root,[-.18,.42,0],[.22,.85,.24],cloth);
  box(root,[.18,.42,0],[.22,.85,.24],cloth);
  box(root,[-.18,.06,.06],[.24,.14,.36],boots);
  box(root,[.18,.06,.06],[.24,.14,.36],boots);
  if(team){box(root,[.32,1.05,.3],[.1,.1,.6],mat(team,.3));}
  root.userData.torso=torso;root.userData.head=head;
  return root;
}

var lobby=new THREE.Group();scene.add(lobby);
var lobbyChar=null,lobbyWeapon=null,lobbyButterfly=null;
function clearGroup(g){while(g.children.length){var c=g.children[0];g.remove(c);}}
function buildLobby(){
  clearGroup(lobby);
  lobbyChar=makeCharacter(CSKINS[S.cskin]);
  lobby.add(lobbyChar);
  var def=WEAPONS[S.weapon];
  var w=(def.cls===5)?makeButterfly(GSKINS[S.gskin]):makeWeapon(def,GSKINS[S.gskin]);
  w.position.set(0,-.1,.5);w.rotation.y=-Math.PI/2;w.rotation.z=.2;
  lobbyChar.userData.torso.add(w);
  lobbyWeapon=w;
  lobbyButterfly=makeButterfly(GSKINS[S.gskin]);
  lobbyButterfly.position.set(-1.6,1.1,.4);lobbyButterfly.scale.setScalar(1.35);
  lobby.add(lobbyButterfly);
}

var SIZE=28,CELL=2,OPEN='.PTAB~';
function walkable(ch){return OPEN.indexOf(ch)>=0;}
function newGrid(){var m=[];for(var y=0;y<SIZE;y++){m[y]=[];for(var x=0;x<SIZE;x++)m[y][x]='#';}return m;}
function room(m,a,b,c,d){for(var y=b;y<=d;y++)for(var x=a;x<=c;x++)if(x>0&&y>0&&x<SIZE-1&&y<SIZE-1)m[y][x]='.';}
function hc(m,y,a,b){for(var x=Math.min(a,b);x<=Math.max(a,b);x++)m[y][x]='.';}
function vc(m,x,a,b){for(var y=Math.min(a,b);y<=Math.max(a,b);y++)m[y][x]='.';}
function put(m,x,y,ch){m[y][x]=ch;}
function buildGrid(index){
  var m=newGrid();
  if(index===0){
    room(m,2,2,8,7);room(m,19,2,25,7);room(m,11,11,16,16);room(m,2,20,8,25);
    room(m,19,20,25,25);room(m,2,12,7,15);room(m,20,12,25,15);room(m,12,2,15,8);room(m,12,19,15,25);
    hc(m,4,8,12);hc(m,4,15,19);hc(m,23,8,12);hc(m,23,15,19);
    vc(m,5,7,12);vc(m,22,7,12);vc(m,5,15,20);vc(m,22,15,20);hc(m,13,7,11);hc(m,13,16,20);
    put(m,5,4,'A');put(m,22,23,'B');put(m,14,14,'P');
    var t=[[5,5],[22,22],[14,4],[14,23],[4,13],[23,13]];
    for(var i=0;i<t.length;i++)put(m,t[i][0],t[i][1],'T');
    var cr=[[4,6],[7,3],[23,4],[20,6],[4,22],[7,25],[23,21],[20,24],[13,12],[14,15]];
    for(i=0;i<cr.length;i++)put(m,cr[i][0],cr[i][1],'X');
    put(m,12,4,'=');put(m,15,23,'=');put(m,6,13,'=');put(m,21,13,'=');
  } else if(index===1){
    room(m,2,2,9,8);room(m,18,2,25,8);room(m,2,19,9,25);room(m,18,19,25,25);room(m,10,10,17,17);
    hc(m,5,9,18);hc(m,22,9,18);vc(m,5,8,19);vc(m,22,8,19);vc(m,13,2,10);vc(m,14,17,25);
    put(m,5,5,'A');put(m,22,22,'B');put(m,13,13,'P');
    var t2=[[6,5],[21,22],[5,22],[22,5],[15,12]];
    for(i=0;i<t2.length;i++)put(m,t2[i][0],t2[i][1],'T');
    var cr2=[[3,3],[8,7],[19,7],[24,3],[3,24],[8,21],[19,21],[24,24]];
    for(i=0;i<cr2.length;i++)put(m,cr2[i][0],cr2[i][1],'X');
    put(m,12,5,'=');put(m,16,22,'=');put(m,5,12,'=');put(m,22,16,'=');
  } else {
    room(m,2,2,12,12);room(m,15,2,25,12);room(m,2,15,12,25);room(m,15,15,25,25);room(m,11,11,16,16);
    hc(m,7,12,15);hc(m,20,12,15);vc(m,7,12,15);vc(m,20,12,15);
    put(m,5,5,'A');put(m,22,22,'B');put(m,13,13,'P');
    var t3=[[6,6],[20,20],[6,20],[20,6],[12,12],[16,15]];
    for(i=0;i<t3.length;i++)put(m,t3[i][0],t3[i][1],'T');
    for(var y=4;y<=10;y+=3){put(m,5,y,'X');put(m,9,y,'X');put(m,18,y,'X');put(m,22,y,'X');}
    for(y=17;y<=23;y+=3){put(m,5,y,'X');put(m,9,y,'X');put(m,18,y,'X');put(m,22,y,'X');}
    put(m,13,9,'=');put(m,13,18,'=');put(m,9,13,'=');put(m,18,13,'=');
  }
  return m;
}
function worldX(cx){return (cx-14)*CELL;}
function worldZ(cy){return (cy-14)*CELL;}
function cellX(x){return Math.round(x/CELL+14);}
function cellZ(z){return Math.round(z/CELL+14);}
function canStand(x,z){var cx=cellX(x),cz=cellZ(z);if(cx<1||cz<1||cx>=SIZE-1||cz>=SIZE-1)return false;return walkable(grid[cz][cx]);}
function lineClear(x1,z1,x2,z2){
  var dx=x2-x1,dz=z2-z1,d=Math.sqrt(dx*dx+dz*dz),steps=Math.max(1,Math.ceil(d/.35));
  for(var i=1;i<steps;i++){var t=i/steps;if(!canStand(x1+dx*t,z1+dz*t))return false;}
  return true;
}
function spawns(ch){var out=[];for(var y=0;y<SIZE;y++)for(var x=0;x<SIZE;x++)if(grid[y][x]===ch)out.push([worldX(x),worldZ(y)]);return out;}

var mapGroup=null,grid=null;
function buildMap(){
  if(mapGroup)scene.remove(mapGroup);
  grid=buildGrid(S.map);
  mapGroup=new THREE.Group();scene.add(mapGroup);
  var wall=mat(0xb99a6c),wallDark=mat(0x8d714c),floor=mat(0xc7a97b),crate=mat(0x6b5134);
  var siteA=mat(0xe2b866),siteB=mat(0x6fbbd1);
  box(mapGroup,[0,-.26,0],[SIZE*CELL,.5,SIZE*CELL],floor);
  for(var y=0;y<SIZE;y++)for(var x=0;x<SIZE;x++){
    var ch=grid[y][x],wx=worldX(x),wz=worldZ(y);
    if(ch==='#'||ch==='X'||ch==='='){
      var h=ch==='X'?1.6:(ch==='='?1.2:3+((x*7+y*13)%5)*.18);
      var w=ch==='X'?1.5:CELL;
      box(mapGroup,[wx,h/2,wz],[w,h,ch==='='?CELL*.5:CELL],ch==='X'||ch==='='?crate:((x+y)%3===0?wallDark:wall));
    } else if(ch==='A'||ch==='B'){
      var disc=cyl(mapGroup,[wx,.03,wz],[2.6,.04,2.6],ch==='A'?siteA:siteB);
      disc.material.transparent=true;disc.material.opacity=.45;
    }
  }
}

var player={x:0,z:0,y:1.0,yaw:0,pitch:0,hp:100,armor:50,vy:0};
var bots=[];
function spawnBots(){
  for(var i=0;i<bots.length;i++)scene.remove(bots[i].obj);
  bots=[];
  var pts=spawns('T'),count=Math.min(S.bots,pts.length);
  for(var j=0;j<count;j++){
    var team=j%2===0?0xb7834e:0x4c6c8c;
    var sk=SK('бот',1,team,team,0xe2b866);
    var obj=makeCharacter(sk,team);
    obj.position.set(pts[j][0],0,pts[j][1]);
    scene.add(obj);
    bots.push({obj:obj,hp:100,next:j*.4,team:team,patrol:[pts[j][0],pts[j][1]]});
  }
}
function botHit(b,dmg){b.hp-=dmg;if(b.hp<=0){S.kills++;save();b.hp=100;var p=spawns('T');var best=p[0],bd=-1;for(var i=0;i<p.length;i++){var d=Math.hypot(p[i][0]-player.x,p[i][1]-player.z);if(d>bd){bd=d;best=p[i];}}b.obj.position.set(best[0],0,best[1]);feed('УБИЙСТВО · '+WEAPONS[S.weapon].name);}}
function hurtPlayer(d){player.armor-=d*.45;player.hp-=d*(player.armor<0?.8:.55);if(player.armor<0)player.armor=0;if(player.hp<=0){player.hp=100;player.armor=50;var p=spawns('P');player.x=p[0][0];player.z=p[0][1];feed('ВЫ УБИТЫ');}}

var state='menu',ammo=0,reserve=0,nextShot=0,reloadEnd=0,recoil=0,timer=115,score=0,escore=0,round=0;
var t=0,feedUntil=0;
var viewWeapon=null;
var fireHeld=false,jumpHeld=false;
var joy={id:-1,x:0,y:0,ox:0,oy:0},look={id:-1,x:0,y:0};
var keys={};

function beep(freq,dur,type){
  if(!S.sound)return;
  try{
    var ctx=beep.ctx||(beep.ctx=new (window.AudioContext||window.webkitAudioContext)());
    var o=ctx.createOscillator(),g=ctx.createGain();
    o.type=type||'square';o.frequency.value=freq;g.gain.value=.06;
    o.connect(g);g.connect(ctx.destination);o.start();g.gain.exponentialRampToValueAtTime(.001,ctx.currentTime+dur);o.stop(ctx.currentTime+dur);
  }catch(e){}
}
function feed(msg){var f=document.getElementById('feed');f.textContent=msg;feedUntil=t+2.2;}

function refreshLobbyStyle(){
  if(state==='menu'){scene.background.setHex(0x14171b);scene.fog.color.setHex(0x14171b);}
}
function buildView(){
  if(viewWeapon){camera.remove(viewWeapon);viewWeapon=null;}
  var def=WEAPONS[S.weapon];
  viewWeapon=(def.cls===5)?makeButterfly(GSKINS[S.gskin]):makeWeapon(def,GSKINS[S.gskin]);
  viewWeapon.position.set(.05,-.34,-.62);
  if(def.cls===3)viewWeapon.position.set(0,-.28,-.5);
  camera.add(viewWeapon);
}
function show(name){
  var inGame=name==='play'||name==='pause';
  if(mapGroup)mapGroup.visible=inGame;
  for(var bi=0;bi<bots.length;bi++)bots[bi].obj.visible=inGame;
  var ids=['menu','weapons','skins','settings','pause'];
  for(var i=0;i<ids.length;i++)document.getElementById(ids[i]).classList.toggle('on',ids[i]===name);
  document.getElementById('hud').classList.toggle('on',name==='play'||name==='pause');
  if(name==='menu'){state='menu';lobby.visible=true;buildLobby();if(viewWeapon)viewWeapon.visible=false;refreshLobbyStyle();updateMenuStats();}
  else if(name==='play'||name==='pause'){state=name;lobby.visible=false;if(viewWeapon)viewWeapon.visible=true;}
  else {state=name;lobby.visible=true;if(viewWeapon)viewWeapon.visible=false;buildLobby();}
}
function updateMenuStats(){document.getElementById('menuStats').innerHTML='УБИЙСТВА '+S.kills+' · ПОБЕДЫ '+S.wins+'<br>v1.0 · 30 оружий · SANDSTONE';}

function startMatch(){
  buildMap();spawnBots();
  var p=spawns('P');player.x=p[0][0];player.z=p[0][1];player.yaw=0;player.pitch=0;player.hp=100;player.armor=50;
  var def=WEAPONS[S.weapon];ammo=def.mag;reserve=def.mag*4;timer=115;score=0;escore=0;
  buildView();scene.background.setHex(0x6b5b45);scene.fog.color.setHex(0x6b5b45);
  show('play');feed('РАУНД НАЧАЛСЯ');
}
function pauseGame(){if(state==='play'){show('pause');}}
function resumeGame(){if(state==='pause'){show('play');}}
function backPressed(){
  if(state==='play')pauseGame();
  else if(state==='pause')resumeGame();
  else if(state!=='menu')show('menu');
}

function shoot(){
  var def=WEAPONS[S.weapon];
  if(t<nextShot||t<reloadEnd||ammo<=0)return;
  nextShot=t+60/Math.max(60,def.rpm)/1000;
  ammo--;recoil=1;beep(def.cls===4?110:180,.08,'sawtooth');
  var dir=new THREE.Vector3();camera.getWorldDirection(dir);
  var pellets=def.cls===4?8:1;
  for(var p=0;p<pellets;p++){
    for(var i=0;i<bots.length;i++){
      var b=bots[i],bx=b.obj.position.x,bz=b.obj.position.z;
      var to=new THREE.Vector3(bx-player.x,.9-player.y,bz-player.z);
      var d=to.length();if(d>def.range*2.2)continue;
      to.normalize();
      var limit=def.spread*.004+(def.cls===4?.045:0);
      if(Math.random()<.35)limit*=.6;
      var spread=(Math.random()-.5)*limit*2;
      var aim=dir.clone();aim.x+=spread;aim.z+=(Math.random()-.5)*limit*2;aim.normalize();
      if(aim.angleTo(to)<limit*.9&&lineClear(player.x,player.z,bx,bz)){
        var head=Math.random()<.18;
        botHit(b,def.dmg*(head?2.4:1));
      }
    }
  }
  if(ammo===0)startReload();
}
function startReload(){if(t<reloadEnd||reserve<=0||ammo>=WEAPONS[S.weapon].mag)return;reloadEnd=t+1.5;setTimeout(function(){var need=WEAPONS[S.weapon].mag-ammo;var take=Math.min(need,reserve);ammo+=take;reserve-=take;},1500);}
function knife(){S.weapon=29;save();buildView();buildLobby();var b=viewWeapon&&viewWeapon.userData.pivots?viewWeapon:null;if(b)b.userData.flip=0;}

function update(dt){
  if(state!=='play')return;
  var def=WEAPONS[S.weapon];
  var mx=joy.x,mz=joy.y;
  if(keys['KeyW']||keys['ArrowUp'])mz+=1;
  if(keys['KeyS']||keys['ArrowDown'])mz-=1;
  if(keys['KeyD']||keys['ArrowRight'])mx+=1;
  if(keys['KeyA']||keys['ArrowLeft'])mx-=1;
  if(keys['ShiftLeft'])mz*=1.25;
  var fx=-Math.sin(player.yaw),fz=-Math.cos(player.yaw);
  var rx=Math.cos(player.yaw),rz=-Math.sin(player.yaw);
  var vx=(fx*mz+rx*mx),vz=(fz*mz+rz*mx);
  var l=Math.hypot(vx,vz);if(l>1){vx/=l;vz/=l;}
  var speed=4.6*def.move;
  var nx=player.x+vx*speed*dt,nz=player.z+vz*speed*dt;
  if(canStand(nx,player.z))player.x=nx;
  if(canStand(player.x,nz))player.z=nz;
  if(jumpHeld&&player.y<=1.01){player.vy=5.4;jumpHeld=false;}
  player.vy-=18*dt;player.y+=player.vy*dt;
  if(player.y<1){player.y=1;player.vy=0;}

  timer-=dt;if(timer<=0){timer=115;if(score>=escore)S.wins++;score=0;escore=0;save();feed('РАУНД ЗАВЕРШЁН');}
  if(fireHeld)shoot();
  if(recoil>0)recoil=Math.max(0,recoil-dt*5);
  if(viewWeapon){
    viewWeapon.position.z=-.62+recoil*.12;
    viewWeapon.position.y=(-.34-recoil*.05)+(def.cls===3?.06:0)+Math.sin(t*2)*.004;
    viewWeapon.rotation.x=-recoil*.16;
    animateButterfly(viewWeapon,dt,recoil>0?14:2.2);
  }

  for(var i=0;i<bots.length;i++){
    var b=bots[i],dx=player.x-b.obj.position.x,dz=player.z-b.obj.position.z,dist=Math.hypot(dx,dz);
    if(dist>.1){
      var ux=dx/dist,uz=dz/dist;
      var side=Math.sin(t*.8+i)*.5;
      var bx=b.obj.position.x+(ux-uz*side)*2.6*dt*(dist>8?1:.4);
      var bz=b.obj.position.z+(uz+ux*side)*2.6*dt*(dist>8?1:.4);
      if(canStand(bx,b.obj.position.z))b.obj.position.x=bx;
      if(canStand(b.obj.position.x,bz))b.obj.position.z=bz;
      b.obj.rotation.y=Math.atan2(dx,dz);
    }
    if(dist<22&&t>b.next&&lineClear(b.obj.position.x,b.obj.position.z,player.x,player.z)){
      b.next=t+1.6-S.diff*.35;
      if(Math.random()<.18+S.diff*.09)hurtPlayer(Math.random()<.12?32:13);
    }
  }

  camera.position.set(player.x,player.y,player.z);
  camera.rotation.set(player.pitch,player.yaw,0,'YXZ');
  updateHud();
}
function updateHud(){
  document.getElementById('hL').innerHTML='ЗДОРОВЬЕ '+Math.round(player.hp)+'<br>БРОНЯ '+Math.round(player.armor);
  var sec=Math.max(0,Math.ceil(timer));
  document.getElementById('hR').textContent=Math.floor(sec/60)+':'+('0'+sec%60).slice(-2)+'   '+score+' : '+escore;
  document.getElementById('feed').style.opacity=t<feedUntil?1:0;
}

function render(now){
  requestAnimationFrame(render);
  var dt=Math.min(.05,(now-last)/1000);last=now;t+=dt;
  if(state==='play')update(dt);
  else{
    if(lobbyChar){lobbyChar.userData.torso.position.y=1.02+Math.sin(t*1.6)*.012;lobbyChar.userData.head.position.y=1.62+Math.sin(t*1.6)*.016;lobbyChar.rotation.y=Math.sin(t*.25)*.35;}
    animateButterfly(lobbyButterfly,dt,2.2);
    camera.position.set(0,1.2,-3.7);
    camera.lookAt(0,1.15,0);
    camera.rotation.z=0;
  }
  renderer.render(scene,camera);
}
var last=performance.now();

function bindUi(){
  var go=document.querySelectorAll('[data-go]');
  for(var i=0;i<go.length;i++){
    go[i].addEventListener('pointerup',function(){
      var v=this.getAttribute('data-go');
      if(v==='play')startMatch();
      else if(v==='resume')resumeGame();
      else show(v);
    });
  }
  var grid=document.getElementById('weaponGrid');
  function renderWeapons(){
    grid.innerHTML='';
    for(var i=0;i<WEAPONS.length;i++){
      (function(idx){
        var d=document.createElement('div');
        d.className='btn'+(idx===S.weapon?' sel':'');d.textContent=WEAPONS[idx].name;
        d.addEventListener('pointerup',function(){S.weapon=idx;save();buildLobby();renderWeapons();renderWeaponStats();});
        grid.appendChild(d);
      })(i);
    }
  }
  function renderWeaponStats(){
    var d=WEAPONS[S.weapon];
    document.getElementById('weaponStats').innerHTML='<b style="color:#E2B866;font-size:2.6vh">'+d.name+'</b> <span class="small">'+d.proto+'</span><br>'+
      CLASS_NAMES[d.cls]+' · УРОН '+d.dmg+' · МАГ '+d.mag+' · СКОРОСТРЕЛЬНОСТЬ '+d.rpm+'/мин<br>РАЗБРОС '+d.spread+' · ОТДАЧА '+d.recoil+' · ДАЛЬНОСТЬ '+d.range+' м';
  }
  renderWeapons();renderWeaponStats();

  var lists=document.getElementById('skinLists');
  function renderSkins(){
    lists.innerHTML='';
    for(var col=0;col<2;col++){
      var wrap=document.createElement('div');wrap.style.display='flex';wrap.style.flexDirection='column';wrap.style.gap='1vh';
      var arr=col===0?CSKINS:GSKINS,key=col===0?'cskin':'gskin';
      var h=document.createElement('div');h.className='small';h.textContent=col===0?'ПЕРСОНАЖ':'ОРУЖИЕ';wrap.appendChild(h);
      for(var i=0;i<arr.length;i++){
        (function(idx,k,a){
          var d=document.createElement('div');
          d.className='btn'+(S[k]===idx?' sel':'');
          d.style.color=RARITY[a[idx].rarity];d.textContent=a[idx].name;
          d.addEventListener('pointerup',function(){S[k]=idx;save();buildLobby();if(k==='gskin')buildView();renderSkins();});
          wrap.appendChild(d);
        })(i,key,arr);
      }
      lists.appendChild(wrap);
    }
  }
  renderSkins();

  function renderSettings(){
    document.getElementById('setBots').textContent='БОТЫ: '+S.bots;
    document.getElementById('setDiff').textContent='СЛОЖНОСТЬ: '+(S.diff===0?'ЛЁГКАЯ':S.diff===1?'СРЕДНЯЯ':'СЛОЖНАЯ');
    document.getElementById('setSens').textContent='ЧУВСТВИТЕЛЬНОСТЬ: '+S.sens+'%';
    document.getElementById('setMap').textContent='КАРТА: '+MAP_NAMES[S.map];
    document.getElementById('setSound').textContent='ЗВУК: '+(S.sound?'ВКЛ':'ВЫКЛ');
  }
  renderSettings();
  var setters=document.querySelectorAll('[data-set]');
  for(i=0;i<setters.length;i++){
    setters[i].addEventListener('pointerup',function(){
      var v=this.getAttribute('data-set');
      if(v==='bots+')S.bots=Math.min(9,S.bots+1);
      if(v==='bots-')S.bots=Math.max(1,S.bots-1);
      if(v==='diff')S.diff=(S.diff+1)%3;
      if(v==='sens+')S.sens=Math.min(220,S.sens+10);
      if(v==='sens-')S.sens=Math.max(40,S.sens-10);
      if(v==='map')S.map=(S.map+1)%3;
      if(v==='sound')S.sound=!S.sound;
      save();renderSettings();
    });
  }

  function hold(id,down,up){
    var el=document.getElementById(id);
    el.addEventListener('pointerdown',function(e){e.preventDefault();down();});
    el.addEventListener('pointerup',function(e){e.preventDefault();if(up)up();});
    el.addEventListener('pointercancel',function(){if(up)up();});
    el.addEventListener('pointerleave',function(){if(up)up();});
  }
  hold('fire',function(){fireHeld=true;},function(){fireHeld=false;});
  hold('jump',function(){jumpHeld=true;});
  document.getElementById('reload').addEventListener('pointerup',function(){startReload();});
  document.getElementById('knife').addEventListener('pointerup',function(){knife();});
  document.getElementById('pause').addEventListener('pointerup',function(){pauseGame();});

  var canvas=renderer.domElement;
  var joyEl=document.getElementById('joy'),knob=document.getElementById('knob');
  canvas.addEventListener('touchstart',function(e){
    if(state!=='play')return;
    e.preventDefault();
    for(var i=0;i<e.changedTouches.length;i++){
      var tt=e.changedTouches[i];
      if(tt.clientX<innerWidth*.45&&joy.id<0){
        joy.id=tt.identifier;joy.ox=tt.clientX;joy.oy=tt.clientY;joy.x=0;joy.y=0;
        joyEl.style.display='block';joyEl.style.left=(tt.clientX-innerHeight*.1)+'px';joyEl.style.top=(tt.clientY-innerHeight*.1)+'px';
      }else if(look.id<0){look.id=tt.identifier;look.x=tt.clientX;look.y=tt.clientY;}
    }
  },{passive:false});
  canvas.addEventListener('touchmove',function(e){
    if(state!=='play')return;
    e.preventDefault();
    for(var i=0;i<e.changedTouches.length;i++){
      var tt=e.changedTouches[i];
      if(tt.identifier===joy.id){
        var dx=(tt.clientX-joy.ox)/(innerHeight*.1),dy=(tt.clientY-joy.oy)/(innerHeight*.1);
        var l=Math.hypot(dx,dy);if(l>1){dx/=l;dy/=l;}
        joy.x=dx;joy.y=-dy;
        knob.style.transform='translate('+dx*innerHeight*.06+'px,'+dy*innerHeight*.06+'px)';
      }else if(tt.identifier===look.id){
        var mdx=tt.clientX-look.x,mdy=tt.clientY-look.y;
        player.yaw-=mdx*.004*S.sens/100;
        player.pitch=Math.max(-.9,Math.min(.9,player.pitch-mdy*.003*S.sens/100));
        look.x=tt.clientX;look.y=tt.clientY;
      }
    }
  },{passive:false});
  function endTouch(e){
    for(var i=0;i<e.changedTouches.length;i++){
      var tt=e.changedTouches[i];
      if(tt.identifier===joy.id){joy.id=-1;joy.x=0;joy.y=0;joyEl.style.display='none';knob.style.transform='';}
      if(tt.identifier===look.id)look.id=-1;
    }
  }
  canvas.addEventListener('touchend',endTouch);canvas.addEventListener('touchcancel',endTouch);

  window.addEventListener('keydown',function(e){keys[e.code]=true;if(e.code==='KeyR')startReload();if(e.code==='KeyQ')knife();if(e.code==='Escape')backPressed();if(e.code==='Space')jumpHeld=true;});
  window.addEventListener('keyup',function(e){keys[e.code]=false;});
  canvas.addEventListener('mousedown',function(e){if(state==='play'&&e.button===0)fireHeld=true;});
  window.addEventListener('mouseup',function(){fireHeld=false;});
  window.addEventListener('mousemove',function(e){if(state!=='play'||document.pointerLockElement!==canvas)return;player.yaw-=e.movementX*.002*S.sens/100;player.pitch=Math.max(-.9,Math.min(.9,player.pitch-e.movementY*.002*S.sens/100));});
  canvas.addEventListener('click',function(){if(state==='play'&&canvas.requestPointerLock)canvas.requestPointerLock();});
  window.addEventListener('resize',function(){camera.aspect=innerWidth/innerHeight;camera.updateProjectionMatrix();renderer.setSize(innerWidth,innerHeight);});
}

buildLobby();bindUi();show('menu');
requestAnimationFrame(render);
})();
