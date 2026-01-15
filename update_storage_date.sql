UPDATE [ExportContainerStoreInfos]
SET StorageDate = CASE 
    WHEN FreeDayStorageDate > '2025-09-01' THEN FreeDayStorageDate 
    ELSE '2025-09-01' 
END
WHERE UserCode = 'YNGNVN' 
  AND isdeleted = 0;
