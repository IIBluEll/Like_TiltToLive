using UnityEngine;
using UnityEditor;
public class SpriteAutoSlicer
{
    // 유니티 상단 메뉴에 버튼을 생성합니다.
    [MenuItem("Tools/Slice Selected Sprites")]
    public static void SliceSelectedSprites()
    {
        // 1. Project 창에서 현재 선택된 텍스처 파일들만 가져옵니다.
        Object[] tSelectedTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

        if ( tSelectedTextures.Length == 0 )
        {
            Debug.LogWarning("[SpriteAutoSlicer] 텍스처 파일을 먼저 선택해주세요.");
            return;
        }

        int tColCount = 20; // 가로 분할 개수
        int tRowCount = 1;  // 세로 분할 개수

        foreach ( Object tObj in tSelectedTextures )
        {
            Texture2D tTexture = tObj as Texture2D;
            string tPath = AssetDatabase.GetAssetPath(tTexture);

            // 텍스처 임포트 설정을 제어하는 Importer를 가져옵니다.
            TextureImporter tImporter = AssetImporter.GetAtPath(tPath) as TextureImporter;

            if ( tImporter == null ) continue;

            // 2. 스프라이트 모드를 Multiple로 강제 변경
            tImporter.spriteImportMode = SpriteImportMode.Multiple;

            // 3. 자를 크기(Rect) 계산
            int tSpriteWidth = tTexture.width / tColCount;
            int tSpriteHeight = tTexture.height / tRowCount;

            SpriteMetaData[] tMetaData = new SpriteMetaData[tColCount * tRowCount];

            for ( int i = 0; i < tColCount; i++ )
            {
                SpriteMetaData tSmd = new SpriteMetaData();
                tSmd.alignment = (int)SpriteAlignment.Center;
                tSmd.pivot = new Vector2(0.5f , 0.5f);
                tSmd.name = $"{tTexture.name}_{i}";

                // 가로로 순차적으로 영역을 지정합니다.
                tSmd.rect = new Rect(i * tSpriteWidth , 0 , tSpriteWidth , tSpriteHeight);
                tMetaData[ i ] = tSmd;
            }

            // 4. 메타데이터 적용 및 저장
            tImporter.spritesheet = tMetaData;
            EditorUtility.SetDirty(tImporter);
            tImporter.SaveAndReimport();
        }

        Debug.Log($"[SpriteAutoSlicer] 총 {tSelectedTextures.Length}개의 이미지 파일이 성공적으로 28등분 되었습니다!");
    }
}
