﻿using DemoProject.Application.Model;

namespace DemoProject.Application.Interface;

public interface ICategoryService
{
    List<Category> GetAll();
    Category? GetById(Guid id);
    bool Create(Category category);
    Category? Update(Category category);
    bool Delete(Guid id);
}