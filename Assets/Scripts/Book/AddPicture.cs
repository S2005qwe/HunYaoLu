using UnityEngine;

public class AddPictrue : Singleton<AddPictrue>
{
    public Book bookScript; // 引用 Book 脚本

    // 定义多个图片数组，用于不同函数调用
    public Sprite[] additionalImages1;
    public Sprite[] additionalImages2;
    public Sprite[] additionalImages3;
    private void Start()
    {
        if (bookScript == null)
        {
            Debug.LogError("Book 脚本引用未设置！");
            return;
        }
    }

    // 函数1：添加 additionalImages1 中的图片
    public void AddImagesFromSet1()
    {
        AddImages(additionalImages1);
    }

    public void AddImagesFromSet2()
    {
        AddImages(additionalImages2);
    }
    public void AddImagesFromSet3()
    {
        AddImages(additionalImages3);
    }






    // 通用添加图片的方法
    private void AddImages(Sprite[] imagesToAdd)
    {
        // 检查是否有要添加的图片
        if (imagesToAdd != null && imagesToAdd.Length > 0)
        {
            int originalLength = bookScript.bookPages.Length;
            // 处理原书没有页面或者只有一页的情况
            if (originalLength == 0)
            {
                bookScript.bookPages = imagesToAdd;
            }
            else if (originalLength == 1)
            {
                Sprite[] combinedImages = new Sprite[1 + imagesToAdd.Length];
                combinedImages[0] = bookScript.bookPages[0];
                for (int i = 0; i < imagesToAdd.Length; i++)
                {
                    combinedImages[i + 1] = imagesToAdd[i];
                }
                bookScript.bookPages = combinedImages;
            }
            else
            {
                // 原书有两页及以上，将新图片插入到倒数第二页
                Sprite[] beforeLastPage = new Sprite[originalLength - 1];
                System.Array.Copy(bookScript.bookPages, 0, beforeLastPage, 0, originalLength - 1);
                Sprite lastPage = bookScript.bookPages[originalLength - 1];

                // 创建新的合并数组
                Sprite[] combinedImages = new Sprite[originalLength + imagesToAdd.Length];
                // 复制原书除最后一页外的内容
                System.Array.Copy(beforeLastPage, 0, combinedImages, 0, beforeLastPage.Length);
                // 复制要添加的图片
                System.Array.Copy(imagesToAdd, 0, combinedImages, beforeLastPage.Length, imagesToAdd.Length);
                // 复制原书最后一页
                combinedImages[combinedImages.Length - 1] = lastPage;

                // 更新 Book 脚本中的 bookPages 数组
                bookScript.bookPages = combinedImages;
            }

            bookScript.UpdateSprites(); // 调用 Book 脚本中的 UpdateSprites 方法来更新显示的图片
        }
        else
        {
            Debug.LogWarning("没有要添加的图片！");
        }
    }
}